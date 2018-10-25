using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Known.Extensions;
using Known.Platform.Business;
using Known.Web;
using Known.WebApi.Extensions;

namespace Known.WebApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiLoginAuthorizeAttribute : AuthorizeAttribute
    {
        private const int ExpiredSeconds = 600;

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.IsUseAttributeOf<AllowAnonymousAttribute>())
                return;

            if (!IsAuthorized(actionContext))
                return;

            if (!ValidateRequest(actionContext))
                return;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = Thread.CurrentPrincipal;
            if (principal == null && HttpContext.Current != null)
                principal = HttpContext.Current.User;

            if (principal != null && principal.Identity != null && !principal.Identity.IsAuthenticated
                && actionContext.IsUseAttributeOf<AuthorizeAttribute>())
            {
                actionContext.CreateErrorResponse("用户未登录！");
                return false;
            }

            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
            {
                if (!(principal.Identity is BasicAuthenticationIdentity identity))
                    return false;

                var context = new Context(identity.Name);
                var bizUser = new UserBusiness(context);
                var result = bizUser.ValidateLogin(identity.Name, identity.Password);
                if (!result.IsValid)
                {
                    actionContext.CreateErrorResponse(result.Message);
                }
                else
                {
                    actionContext.RequestContext.Principal = principal;
                    actionContext.Request.Properties["Known_User"] = result.Data;
                }
                return result.IsValid;
            }

            return false;
        }

        private static bool ValidateRequest(HttpActionContext actionContext)
        {
            var timestamp = actionContext.Request.GetQueryValue("timestamp");
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                actionContext.CreateErrorResponse("缺少参数timestamp！");
                return false;
            }

            if (!long.TryParse(timestamp, out long ms) || ms.ToString().Length != 13)
            {
                actionContext.CreateErrorResponse("不合法的timestamp！");
                return false;
            }

            var requestTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(ms);
            var diffSeconds = (DateTime.Now - requestTime).TotalSeconds;
            if (diffSeconds > ExpiredSeconds || diffSeconds < 0 - ExpiredSeconds)
            {
                actionContext.CreateErrorResponse("请求已超时！");
                return false;
            }

            var nonce = actionContext.Request.GetQueryValue("nonce");
            if (string.IsNullOrWhiteSpace(nonce))
            {
                actionContext.CreateErrorResponse("缺少参数nonce！");
                return false;
            }

            var sign = actionContext.Request.GetQueryValue("sign");
            if (string.IsNullOrWhiteSpace(sign))
            {
                actionContext.CreateErrorResponse("缺少参数sign！");
                return false;
            }

            if (sign != GetSignature(actionContext.Request))
            {
                actionContext.CreateErrorResponse("sign格式不正确！");
                return false;
            }

            return true;
        }

        private static string GetSignature(HttpRequestMessage request)
        {
            var args = new Dictionary<string, object>();
            var pairs = request.GetQueryNameValuePairs();
            foreach (var item in pairs)
            {
                if (item.Key != "sign")
                    args.Add(item.Key, item.Value);
            }

            if (request.Method.Method == "POST")
            {
                var stream = request.Content.ReadAsStreamAsync().Result;
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                args.Add("body", request.Content.ReadAsStringAsync().Result);
            }

            return args.ToMd5Signature();
        }
    }
}