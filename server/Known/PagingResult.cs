namespace Known
{
    public class PagingResult
    {
        public PagingResult(int totalCount, object pageData)
        {
            TotalCount = totalCount;
            PageData = pageData;
        }

        public int TotalCount { get; }
        public object PageData { get; }
    }
}
