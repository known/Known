﻿namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 设置自动查询条件。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="criteria">查询条件对象。</param>
    public void SetAutoQuery<T>(ref string sql, PagingCriteria criteria)
    {
        QueryHelper.SetAutoQuery<T>(this, ref sql, criteria);
    }

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria, Func<T, ExportColumnInfo, object> onExport = null) where T : class, new()
    {
        var sb = Provider?.Sql.SelectAll().From<T>();
        if (typeof(T).IsAssignableFrom(typeof(EntityBase)))
            sb?.Where(nameof(EntityBase.CompNo));
        var sql = sb?.ToSqlString();
        return QueryPageAsync(sql, criteria, onExport);
    }

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria, Func<T, ExportColumnInfo, object> onExport = null) where T : class, new()
    {
        SetAutoQuery<T>(ref sql, criteria);
        var info = Provider?.GetCommand(sql, criteria, User);
        return QueryPageAsync(info, criteria, onExport);
    }

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="info">查询命令信息。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<PagingResult<T>> QueryPageAsync<T>(CommandInfo info, PagingCriteria criteria, Func<T, ExportColumnInfo, object> onExport = null) where T : class, new()
    {
        try
        {
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            if (conn != null && conn.State != ConnectionState.Open)
                conn.Open();

            Provider?.SetCommand(info, criteria, User);
            byte[] exportData = null;
            Dictionary<string, object> statis = null;
            var watch = Stopwatcher.Start<T>();
            var total = await Task.Run(async () =>
            {
                var cmd = await PrepareCommandAsync(info);
                cmd.CommandText = info.CountSql;
                var value = cmd.ExecuteScalar();
                return Utils.ConvertTo<int>(value);
            });
            var pageData = await Task.Run(async () =>
            {
                var data = new List<T>();
                var cmd = await PrepareCommandAsync(info);
                cmd.CommandText = info.PageSql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = DbUtils.ConvertTo<T>(reader);
                        data.Add((T)obj);
                    }
                }
                return data;
            });
            if (criteria.ExportMode == ExportMode.None)
            {
                if (criteria.StatisColumns != null && criteria.StatisColumns.Count > 0)
                {
                    statis = await Task.Run(async () =>
                    {
                        Dictionary<string, object> data = null;
                        var cmd = await PrepareCommandAsync(info);
                        cmd.CommandText = info.StatSql;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader != null && reader.Read())
                                data = DbUtils.GetDictionary(reader);
                        }
                        return data;
                    });
                }
            }

            if (conn != null && conn.State != ConnectionState.Closed)
                conn.Close();

            if (criteria.ExportMode != ExportMode.None)
                exportData = DbUtils.GetExportData(criteria, pageData, onExport);

            if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
                pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
            
            watch.Watch("PagingResult");
            watch.WriteLog();
            return new PagingResult<T>(total, pageData) { ExportData = exportData, Statis = statis };
        }
        catch (Exception ex)
        {
            HandException(info, ex);
            return new PagingResult<T>();
        }
    }
}