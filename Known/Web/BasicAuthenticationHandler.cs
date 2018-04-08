using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Known.Web
{
    /// <summary>
    /// 基本身份认证处理者。
    /// </summary>
    public class BasicAuthenticationHandler : DelegatingHandler
    {
        private const string AuthenticationHeader = "WWW-Authenticate";

        /// <summary>
        /// 异步发送Http请求到要发送到服务器的内部处理程序。
        /// </summary>
        /// <param name="request">要发送到服务器的Http请求消息。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var identity = ParseAuthenticationHeader(request);
            if (identity != null)
            {
                var principal = new GenericPrincipal(identity, null);
                Thread.CurrentPrincipal = principal;

                //针对于ASP.NET设置
                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                if (identity == null && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Challenge(request, response);
                }

                return response;
            });
        }

        void Challenge(HttpRequestMessage request, HttpResponseMessage response)
        {
            var host = request.RequestUri.DnsSafeHost;
            response.Headers.Add(AuthenticationHeader, string.Format("Basic realm=\"{0}\"", host));
        }

        private BasicAuthenticationIdentity ParseAuthenticationHeader(HttpRequestMessage requestMessage)
        {
            var authParameter = string.Empty;
            var authValue = requestMessage.Headers.Authorization;
            if (authValue != null && authValue.Scheme == "Basic")
                authParameter = authValue.Parameter;

            if (string.IsNullOrEmpty(authParameter))
                return null;

            authParameter = Encoding.Default.GetString(Convert.FromBase64String(authParameter));

            var authToken = authParameter.Split(':');
            if (authToken.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(authToken[0], authToken[1]);
        }
    }
}
