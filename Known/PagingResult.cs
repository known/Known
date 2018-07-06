using System.Collections.Generic;

namespace Known
{
    public class PagingResult<T>
    {
        public PagingResult(int totalCount, List<T> pageData)
        {
            TotalCount = totalCount;
            PageData = pageData;
        }

        public int TotalCount { get; }
        public List<T> PageData { get; }
    }
}
