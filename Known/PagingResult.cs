using System.Collections.Generic;
using System.Data;

namespace Known
{
    public class PagingResult
    {
        public PagingResult(int totalCount, DataTable pageData)
        {
            TotalCount = totalCount;
            PageData = pageData;
        }

        public int TotalCount { get; }
        public DataTable PageData { get; }
    }

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
