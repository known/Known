using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Known.Web
{
    public class ControllerBase : Controller
    {
        public PlatformService Platform => new PlatformService();

        public ControllerBase()
        {
            ViewBag.AppName = Config.App.AppName;
        }

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        public UserInfo CurrentUser
        {
            get { return Platform.GetUserInfo(UserName); }
        }

        protected ActionResult ViewResult()
        {
            return View();
        }

        protected ActionResult PartialResult(string viewName)
        {
            return PartialView(viewName);
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

        protected ActionResult ExportResult(byte[] fileContents, string fileDownloadName, string contentType = "application/octet-stream")
        {
            if (fileDownloadName.ToLower().EndsWith(".pdf"))
                contentType = "application/pdf";
            else if (fileDownloadName.ToLower().EndsWith(".xlsx"))
                contentType = "application/vnd.ms-excel";

            var token = Request.Form["downloadToken"];
            Response.SetCookie(new HttpCookie("downloadToken", token));
            return File(fileContents, contentType, fileDownloadName);
        }

        protected ActionResult QueryPagingData<T>(CriteriaData data, Func<PagingCriteria, PagingResult<T>> func)
        {
            var criteria = data.ToPagingCriteria();
            criteria.Parameter.CompNo = CurrentUser.CompNo;
            var result = func(criteria);
            return JsonResult(new
            {
                code = 0,
                msg = "",
                count = result.TotalCount,
                data = result.PageData,
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
        public string field { get; set; }
        public string order { get; set; }

        public PagingCriteria ToPagingCriteria()
        {
            var orderBys = new List<string>();
            if (!string.IsNullOrWhiteSpace(field) && !string.IsNullOrWhiteSpace(order))
            {
                var sorts = field.Split(',');
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

            if (string.IsNullOrWhiteSpace(query))
                query = "{}";

            criteria.Parameter = Utils.FromJson<dynamic>(query);
            return criteria;
        }
    }
}
