using System.Collections.Generic;

namespace Known
{
    /// <summary>
    /// 分页查询结果。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// 构造函数，创建一个分页查询结果实例。
        /// </summary>
        /// <param name="totalCount">查询总记录数。</param>
        /// <param name="pageData">分页数据集合。</param>
        public PagingResult(int totalCount, List<T> pageData)
        {
            TotalCount = totalCount;
            PageData = pageData;
        }

        /// <summary>
        /// 取得查询总记录数。
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// 取得分页数据集合。
        /// </summary>
        public List<T> PageData { get; }
    }
}
