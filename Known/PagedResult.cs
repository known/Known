using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known
{
    public class PagedResult<T>
    {
        public PagedResult(int totalCount, List<T> dataSource)
        {
            TotalCount = totalCount;
            DataSource = dataSource;
        }

        public int TotalCount { get; private set; }
        public List<T> DataSource { get; private set; }
    }
}
