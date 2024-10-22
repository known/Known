namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria, Func<T, ExportColumnInfo, object> onExport = null) where T : class, new()
    {
        var tableName = Provider.GetTableName<T>();
        var sql = $"select * from {tableName}";

        if (typeof(T).IsAssignableFrom(typeof(EntityBase)))
        {
            var compNo = nameof(EntityBase.CompNo);
            var compName = Provider.FormatName(compNo);
            sql += $" where {compName}=@{compNo}";
        }
        else
        {
            sql += " where 1=1";
        }

        return QueryPageAsync<T>(sql, criteria, onExport);
    }

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria, Func<T, ExportColumnInfo, object> onExport = null) where T : class, new()
    {
        try
        {
            QueryHelper.SetAutoQuery<T>(this, ref sql, criteria);
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            if (conn != null && conn.State != ConnectionState.Open)
                conn.Open();

            byte[] exportData = null;
            Dictionary<string, object> statis = null;
            var watch = Stopwatcher.Start<T>();
            var pageData = new List<T>();
            var info = Provider.GetCommand(sql, criteria, User);
            var cmd = await PrepareCommandAsync(info);
            cmd.CommandText = info.CountSql;
            var value = cmd.ExecuteScalar();
            var total = Utils.ConvertTo<int>(value);
            watch.Watch("Total");
            if (total > 0)
            {
                cmd.CommandText = info.PageSql;
                watch.Watch("Paging");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = DbUtils.ConvertTo<T>(reader);
                        pageData.Add((T)obj);
                    }
                }
                watch.Watch("Convert");
                if (criteria.ExportMode == ExportMode.None)
                {
                    if (criteria.StatisColumns != null && criteria.StatisColumns.Count > 0)
                    {
                        cmd.CommandText = info.StatSql;
                        watch.Watch("Suming");
                        using (var reader1 = cmd.ExecuteReader())
                        {
                            if (reader1 != null && reader1.Read())
                                statis = DbUtils.GetDictionary(reader1);
                        }
                        watch.Watch("Sum");
                    }
                }
            }

            cmd.Parameters.Clear();
            if (conn != null && conn.State != ConnectionState.Closed)
                conn.Close();

            if (criteria.ExportMode != ExportMode.None)
            {
                exportData = DbUtils.GetExportData(criteria, pageData, onExport);
                watch.Watch("Export");
            }

            if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
                pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

            watch.WriteLog();
            return new PagingResult<T>(total, pageData) { ExportData = exportData, Statis = statis };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            Logger.Error(sql);
            return new PagingResult<T>();
        }
    }
}