using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Known.Core.Services;
using Newtonsoft.Json;

namespace Known.Web
{
    public class ControllerBase : Controller
    {
        public PlatformService Platform { get; } = new PlatformService();

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        protected ActionResult JsonResult(object value)
        {
            var json = Utils.ToJson(value);
            return Content(json, "application/json");
        }

        protected ActionResult ErrorResult(string message, object data = null)
        {
            return JsonResult(new { ok = false, message, data });
        }

        protected ActionResult SuccessResult(string message, object data = null)
        {
            return JsonResult(new { ok = true, message, data });
        }

        protected ActionResult ValidateResult(Result result)
        {
            if (!result.IsValid)
                return ErrorResult(result.Message, result.Data);

            return SuccessResult(result.Message, result.Data);
        }

        protected ActionResult PagingResult<T>(PagingResult<T> result)
        {
            return JsonResult(new
            {
                count = result.TotalCount,
                data = result.PageData
            });
        }

        protected ActionResult QueryPagingData<T>(CriteriaData data, Func<PagingCriteria, PagingResult<T>> func)
        {
            var criteria = data.ToPagingCriteria();
            var result = func(criteria);
            return Json(new
            {
                total = result.TotalCount,
                rows = result.PageData,
                summary = result.Summary
            });
        }

        protected ActionResult PostAction<T>(string data, Func<T, Result> func)
        {
            var obj = Utils.FromJson<T>(data);
            if (obj == null)
                return ErrorResult("不能提交空数据！");

            var result = func(obj);
            return ValidateResult(result);
        }
    }

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

            return new PagingCriteria
            {
                PageIndex = page,
                PageSize = limit,
                OrderBys = orderBys.ToArray(),
                Parameter = JsonConvert.DeserializeObject<dynamic>(query)
            };
        }
    }
}
