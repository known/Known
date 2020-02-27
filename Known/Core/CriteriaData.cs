using System.Collections.Generic;
using Newtonsoft.Json;

namespace Known.Core
{
    /// <summary>
    /// 查询条件类。
    /// </summary>
    public class CriteriaData
    {
        /// <summary>
        /// 取得或设置查询参数JSON。
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// 取得或设置是否挂载页面。
        /// </summary>
        public bool isLoad { get; set; }

        /// <summary>
        /// 取得或设置排序字段。
        /// </summary>
        public string sortField { get; set; }

        /// <summary>
        /// 取得或设置排序方式。
        /// </summary>
        public string sortOrder { get; set; }

        /// <summary>
        /// 取得或设置页面。
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 取得或设置每页大小。
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 获取分页查询条件对象。
        /// </summary>
        /// <returns>分页查询条件对象。</returns>
        public PagingCriteria ToPagingCriteria()
        {
            var sorts = new List<string>();
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                sorts.Add($"{sortField} {sortOrder}");
            }

            return new PagingCriteria
            {
                IsLoad = isLoad,
                PageIndex = pageIndex,
                PageSize = pageSize,
                OrderBys = sorts.ToArray(),
                Parameter = FromJson(query)
            };
        }

        private static dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }
    }
}