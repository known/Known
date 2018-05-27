using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Known.Log;
using Known.Web.Extensions;

namespace Known.Web.Api
{
    public class RequestTracker
    {
        private static readonly Logger logger = new TraceLogger(HttpRuntime.AppDomainAppPath);

        public string ClientIp { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double TotalMilliseconds { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string HttpMethod { get; set; }
        public string Headers { get; set; }
        public string Parameters { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }

        internal void Start(HttpActionContext actionContext)
        {
            ClientIp = actionContext.Request.GetClientIP();
            StartTime = DateTime.Now;
            ControllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            ActionName = actionContext.ActionDescriptor.ActionName;
            HttpMethod = actionContext.Request.Method.Method;
            Headers = TrackHelper.GetHeaders(actionContext.Request);
            Parameters = TrackHelper.GetParameters(actionContext.Request);
        }

        internal void Complete(HttpActionExecutedContext actionExecutedContext)
        {
            EndTime = DateTime.Now;
            TotalMilliseconds = (EndTime - StartTime).TotalMilliseconds;
            Result = actionExecutedContext.Response == null ? string.Empty : actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            Error = actionExecutedContext.Exception == null ? "无" : actionExecutedContext.Exception.ToString();

            var info = string.Format(@"{0}
客户端IP：{1}
开始时间：{2:yyyy-MM-dd HH:mm:ss.fff}
结束时间：{3:yyyy-MM-dd HH:mm:ss.fff}
总耗时：{4}毫秒
Controller名称：{5}
Action名称：{6}
请求方式：{7}
请求头：{8}
请求参数：{9}
响应结果：{10}
异常信息：{11}",
new string('-', 100), ClientIp, StartTime, EndTime, TotalMilliseconds,
ControllerName, ActionName, HttpMethod, Headers, Parameters, Result, Error);
            logger.Info(info);
        }
    }
}