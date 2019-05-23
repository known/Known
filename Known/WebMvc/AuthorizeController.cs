using System.Collections.Generic;
using Known.Web.Extensions;
using Known.WebMvc.Filters;
using Newtonsoft.Json;

namespace Known.WebMvc
{
    [LoginAuthorize]
    public class AuthorizeController : BaseController
    {
        protected PagingCriteria GetPagingCriteria()
        {
            var query = Request.Get<string>("query");
            var isLoad = Request.Get<string>("isLoad");
            var sortField = Request.Get<string>("sortField");
            var sortOrder = Request.Get<string>("sortOrder");
            var sorts = new List<string>();
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                sorts.Add($"{sortField} {sortOrder}");
            }

            return new PagingCriteria
            {
                IsLoad = isLoad == "1",
                PageIndex = Request.Get<int>("pageIndex"),
                PageSize = Request.Get<int>("pageSize"),
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