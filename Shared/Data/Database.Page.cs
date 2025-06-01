namespace Known.Data;

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
    /// <returns>分页查询结果。</returns>
    public virtual QueryPageBuilder Query<T>(PagingCriteria criteria) where T : class, new()
    {
        var sb = Provider?.Sql.SelectAll().From<T>();
        if (typeof(T).IsAssignableFrom(typeof(EntityBase)))
            sb?.Where(nameof(EntityBase.CompNo));
        return new QueryPageBuilder(this) { Sql = sb?.ToSqlString(), Criteria = criteria };
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
        return Query<T>(criteria).ToPageAsync(onExport);
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
        if (string.IsNullOrWhiteSpace(sql))
            return Task.FromResult(new PagingResult<T>());

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
            if (!criteria.IsPaging)
                criteria.PageIndex = -1;
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            if (conn != null && conn.State != ConnectionState.Open)
                conn.Open();

            Provider?.SetCommand(info, criteria, User);
            byte[] exportData = null;
            Dictionary<string, object> statis = null;
            var pageData = new List<T>();
            var watch = Stopwatcher.Start<T>();
            var cmd = await PrepareCommandAsync(info);
            cmd.CommandText = info.CountSql;
            var value = cmd.ExecuteScalar();
            var total = Utils.ConvertTo<int>(value);
            if (total > 0)
            {
                cmd.CommandText = info.PageSql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = DbUtils.ConvertTo<T>(reader);
                        criteria.OnDataItem?.Invoke(obj);
                        pageData.Add((T)obj);
                    }
                }
                if (criteria.ExportMode == ExportMode.None)
                {
                    if (criteria.StatisticColumns != null && criteria.StatisticColumns.Count > 0)
                    {
                        cmd.CommandText = info.StatSql;
                        using var reader1 = cmd.ExecuteReader();
                        if (reader1 != null && reader1.Read())
                            statis = DbUtils.GetDictionary(reader1);
                    }
                }
            }

            if (conn != null && conn.State != ConnectionState.Closed)
                conn.Close();

            if (criteria.ExportMode != ExportMode.None)
                exportData = DbUtils.GetExportData(criteria, pageData, onExport);

            if (pageData.Count > criteria.PageSize && criteria.PageSize > 0 && criteria.PageIndex > 0)
                pageData = [.. pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize)];

            watch.Watch("PagingResult");
            watch.WriteLog();
            return new PagingResult<T>(total, pageData) { ExportData = exportData, Statis = statis };
        }
        catch (Exception ex)
        {
            HandException(info, ex);
            return new PagingResult<T>() { Message = ex.Message };
        }
    }
}

/// <summary>
/// 分页查询建造者类。
/// </summary>
/// <param name="database">数据库访问对象。</param>
public class QueryPageBuilder(Database database)
{
    internal string Sql { get; set; }
    internal PagingCriteria Criteria { get; set; }

    /// <summary>
    /// 查询分页结果。
    /// </summary>
    /// <typeparam name="TItem">分页数据类型。</typeparam>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns></returns>
    public Task<PagingResult<TItem>> ToPageAsync<TItem>(Func<TItem, ExportColumnInfo, object> onExport = null) where TItem : class, new()
    {
        return database.QueryPageAsync(Sql, Criteria, onExport);
    }
}