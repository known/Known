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
    /// 用户身份认证处理者类。
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        private const string AuthenticationHeader = "WWW-Authenticate";
        private readonly string authType;

        /// <summary>
        /// 初始化一个用户身份认证处理者类的实例。
        /// </summary>
        /// <param name="authType">用户身份认证类型，Basic 或 Token。</param>
        public AuthenticationHandler(string authType)
        {
            this.authType = authType;
        }

        /// <summary>
        /// 异步发送 HTTP 请求到要发送到服务器的内部处理程序。
        /// </summary>
        /// <param name="request">要发送到服务器的 HTTP 请求消息。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>表示异步操作的任务对象。</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var identity = ParseAuthenticationHeader(request);
            if (identity != null)
            {
                var principal = new GenericPrincipal(identity, null);
                Thread.CurrentPrincipal = principal;

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
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
            if (authType == "Basic")
            {
                var host = request.RequestUri.DnsSafeHost;
                response.Headers.Add(AuthenticationHeader, $"Basic realm=\"{host}\"");
            }
            else
            {
                response.Content = new StringContent("Unauthorized!");
            }
        }

        private AuthenticationIdentity ParseAuthenticationHeader(HttpRequestMessage requestMessage)
        {
            var parameter = string.Empty;
            var authValue = requestMessage.Headers.Authorization;
            if (authValue != null && authValue.Scheme == authType)
            {
                parameter = authValue.Parameter;
            }

            if (string.IsNullOrEmpty(parameter))
                return null;

            AuthenticationIdentity identity = null;
            if (authType == "Basic")
            {
                parameter = Encoding.Default.GetString(Convert.FromBase64String(parameter));
                var authToken = parameter.Split(':');
                if (authToken.Length < 2)
                    return null;

                identity = new AuthenticationIdentity(authToken[0], authType);
                identity.Password = authToken[1];
            }
            else if (authType == "Token")
            {
                identity = new AuthenticationIdentity(parameter, authType);
                identity.Token = parameter;
            }

            return identity;
        }
    }
}
