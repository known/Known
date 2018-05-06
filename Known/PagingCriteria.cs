namespace Known
{
    /// <summary>
    /// 分页查询条件。
    /// </summary>
    public class PagingCriteria
    {
        /// <summary>
        /// 取得或设置分页大小，默认10。
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 取得或设置查询页码，默认1。
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 取得或设置排序字段和方式数组。
        /// </summary>
        public string[] OrderBys { get; set; }

        /// <summary>
        /// 取得或设置查询参数。
        /// </summary>
        public dynamic Parameters { get; set; }
    }
}
