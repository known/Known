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
        /// 取得或设置查询关键字。
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 取得或设置高级查询参数JSON。
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// 取得或设置页码。
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 取得或设置每页大小。
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 取得或设置排序字段，多个用逗号分隔。
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// 取得或设置排序方式，多个用逗号分隔。
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 获取分页查询条件对象。
        /// </summary>
        /// <returns>分页查询条件对象。</returns>
        public PagingCriteria ToPagingCriteria()
        {
            var orderBys = new List<string>();
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sorts = sort.Split(',');
                var orders = order.Split(',');
                for (int i = 0; i < sorts.Length; i++)
                {
                    orderBys.Add($"{sorts[i]} {orders[i]}");
                }
            }

            return new PagingCriteria
            {
                Key = key,
                PageIndex = page,
                PageSize = rows,
                OrderBys = orderBys.ToArray(),
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