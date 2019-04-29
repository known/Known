using System.Collections.Generic;
using Newtonsoft.Json;

namespace Known.WebApi
{
    public class CriteriaData
    {
        public string query { get; set; }
        public bool isLoad { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

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