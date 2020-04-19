using System.Collections.Generic;
using Newtonsoft.Json;

namespace Known.Web
{
    public class CriteriaData
    {
        public string query { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string sort { get; set; }
        public string order { get; set; }

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

            var criteria = new PagingCriteria
            {
                PageIndex = page,
                PageSize = limit,
                OrderBys = orderBys.ToArray()
            };

            if (!string.IsNullOrWhiteSpace(query))
                criteria.Parameter = JsonConvert.DeserializeObject<dynamic>(query);

            return criteria;
        }
    }
}
