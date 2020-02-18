namespace Known
{
    /// <summary>
    /// 分页查询结果类。
    /// </summary>
    public class PagingResult
    {
        /// <summary>
        /// 初始化一个分页查询结果类实例。
        /// </summary>
        /// <param name="totalCount">查询结果总条数。</param>
        /// <param name="pageData">当前页数据集合。</param>
        /// <param name="summary">当前查询结果的摘要数据。</param>
        public PagingResult(int totalCount, object pageData, object summary = null)
        {
            TotalCount = totalCount;
            PageData = pageData;
            Summary = summary;
        }

        /// <summary>
        /// 取得查询结果总条数。
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// 取得当前页数据集合。
        /// </summary>
        public object PageData { get; }

        /// <summary>
        /// 取得当前查询结果的摘要数据。
        /// </summary>
        public object Summary { get; }
    }
}
