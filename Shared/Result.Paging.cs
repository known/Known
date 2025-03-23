namespace Known;

/// <summary>
/// 泛型数据分页查询结果类。
/// </summary>
/// <typeparam name="T">泛型类型。</typeparam>
public class PagingResult<T>
{
    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    public PagingResult() { }

    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    /// <param name="pageData">分页数据列表。</param>
    public PagingResult(List<T> pageData)
    {
        TotalCount = pageData?.Count ?? 0;
        PageData = pageData;
    }

    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    /// <param name="totalCount">总记录数。</param>
    /// <param name="pageData">分页数据列表。</param>
    /// <param name="summary">统计摘要对象。</param>
    public PagingResult(int totalCount, List<T> pageData, object summary = null)
    {
        TotalCount = totalCount;
        PageData = pageData;
        Summary = summary;
    }

    /// <summary>
    /// 取得或设置总记录数。
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 取得或设置分页数据列表。
    /// </summary>
    public List<T> PageData { get; set; }

    /// <summary>
    /// 取得或设置统计字段字典。
    /// </summary>
    public Dictionary<string, object> Statis { get; set; }

    /// <summary>
    /// 取得或设置统计摘要对象。
    /// </summary>
    public object Summary { get; set; }

    /// <summary>
    /// 取得或设置导出数据字节数组。
    /// </summary>
    public byte[] ExportData { get; set; }

    /// <summary>
    /// 获取泛型统计摘要对象。
    /// </summary>
    /// <typeparam name="TSummary">摘要泛型类型。</typeparam>
    /// <returns>泛型统计摘要对象。</returns>
    public TSummary SummaryAs<TSummary>()
    {
        if (Summary == null)
            return default;

        if (Summary is TSummary data)
            return data;

        var dataString = Summary.ToString();
        return Utils.FromJson<TSummary>(dataString);
    }
}