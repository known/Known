namespace Known
{
    /// <summary>
    /// 分页查询条件类。
    /// </summary>
    public class PagingCriteria
    {
        /// <summary>
        /// 取得或设置查询关键字。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 取得或设置每页显示的数据数量，默认 10。
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 取得或设置当前页码，默认 1。
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 取得或设置排序字段及方式。
        /// </summary>
        public string[] OrderBys { get; set; }

        /// <summary>
        /// 取得或设置查询条件动态参数。
        /// </summary>
        public dynamic Parameter { get; set; }
    }
}
