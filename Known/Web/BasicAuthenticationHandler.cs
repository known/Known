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
    public class BasicAuthenticationHandler : DelegatingHandler
    {
        private const string AuthenticationHeader = "WWW-Authenticate";

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
