namespace Known
{
    public class PagingCriteria
    {
        public bool IsLoad { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
        public string[] OrderBys { get; set; }
        public dynamic Parameter { get; set; }
    }
}
