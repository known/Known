using System;
using System.Web;
using System.Web.Mvc;
using Known.Core;

namespace Known.Web
{
    public class ControllerBase : Controller
    {
        public PlatformService Platform { get; } = new PlatformService();

        public ControllerBase()
        {
            ViewBag.AppName = Config.AppName;
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
#if (DEBUG)
            return View();
#else
            var context = GetViewContext();
            var content = ResViewEngine.GetView(context);
            return Content(content, "text/html");
#endif
        }

        protected ActionResult PartialResult(string viewName)
        {
#if (DEBUG)
            return PartialView(viewName);
#else
            var context = GetViewContext(viewName);
            var content = ResViewEngine.GetView(context);
            return Content(content, "text/html");
#endif
        }

#if (!DEBUG)
        private ViewContext GetViewContext(string partialName = null)
        {
            var ctx = ControllerContext;
            var context = new ViewContext
            {
                LayoutAssembly = typeof(ControllerBase).Assembly,
                Assembly = ctx.Controller.GetType().Assembly,
                HttpContext = ctx.HttpContext,
                PartialName = partialName
            };

            context.Controller = ctx.RouteData.Values["controller"].ToString();
            context.Action = ctx.RouteData.Values["action"].ToString();
            return context;
        }
#endif

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
}
