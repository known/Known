using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Known.Log;
using Known.Web.Extensions;

namespace Known.WebApi
{
    public class RequestTracker
    {
        private static readonly Logger log = new FileLogger();

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

            var line = new string('-', 100);
            var info = $@"{line}
客户端IP：{ClientIp}
开始时间：{StartTime}
结束时间：{EndTime}
总耗时：{TotalMilliseconds}毫秒
Action：{ControllerName}.{ActionName}
请求方式：{HttpMethod}
请求头：{Headers}
请求参数：{Parameters}
响应结果：{Result}
异常信息：{Error}";
            log.Info(info);
        }
    }
}