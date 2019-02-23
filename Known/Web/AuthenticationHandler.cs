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
    public class AuthenticationHandler : DelegatingHandler
    {
        private const string AuthenticationHeader = "WWW-Authenticate";
        private string authType;

        public AuthenticationHandler(string authType)
        {
            this.authType = authType;
        }

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
            response.Headers.Add(AuthenticationHeader, $"Basic realm=\"{host}\"");
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
