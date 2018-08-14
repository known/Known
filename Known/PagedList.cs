using System;
using System.Collections.Generic;
using System.Linq;

namespace Known
{
    public interface IPagedList<T> : IList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int PageCount { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    [Serializable]
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int total = source.Count();
            TotalCount = total;
            PageCount = total / pageSize;

            if (total % pageSize > 0)
                PageCount++;

            PageSize = pageSize;
            PageIndex = pageIndex;
            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            TotalCount = source.Count();
            PageCount = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                PageCount++;

            PageSize = pageSize;
            PageIndex = pageIndex;
            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            PageCount = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                PageCount++;

            PageSize = pageSize;
            PageIndex = pageIndex;
            AddRange(source);
        }

        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int PageCount { get; }

        public bool HasPreviousPage
        {
            get { return PageIndex > 0; }
        }

        public bool HasNextPage
        {
            get { return PageIndex + 1 < PageCount; }
        }
    }
}
