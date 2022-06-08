#if !NET6_0
using System.Web;
using System.Web.SessionState;

namespace Known.Web
{
    class SessionHandler : IHttpHandler, IRequiresSessionState
    {
        public static readonly SessionHandler Instance = new SessionHandler();

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable { get; }
    }
}
#endif