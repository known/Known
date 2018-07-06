using System.Web;
using System.Web.Http;
using Known.Log;

namespace Known.WebApi
{
    public class BaseApiController : ApiController, IController
    {
        private Context context;
        public Context Context
        {
            get
            {
                if (context == null)
                {
                    var database = Config.GetDatabase();
                    var logger = new TraceLogger(HttpRuntime.AppDomainAppPath);
                    context = new Context(database, logger, UserName);
                }
                return context;
            }
        }

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        public bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public T LoadBusiness<T>() where T : BusinessBase
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}