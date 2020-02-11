using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace Known.Web.Mvc
{
    public abstract class Controller : IHttpHandler
    {
        private HttpContext context;

        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            User = context.User;
            Session = context.Session;
        }

        public bool IsReusable { get; }

        public IPrincipal User { get; private set; }
        public HttpSessionState Session { get; private set; }

        protected ActionResult Redirect(string url)
        {
            context.Response.Redirect(url);
            return new ActionResult();
        }

        protected ActionResult Content(string content, string mimeType)
        {
            return new ActionResult();
        }
    }
}
