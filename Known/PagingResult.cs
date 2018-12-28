using System.Collections.Generic;

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

    public class PagingResult<T> : PagingResult
    {
        public PagingResult(int totalCount, List<T> pageData)
            : base(totalCount, null)
        {
            PageData = pageData;
        }

        public new List<T> PageData { get; }
    }
}
