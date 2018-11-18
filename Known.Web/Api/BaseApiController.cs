using System.Web.Http;
using Known.Data;
using Known.Log;

namespace Known.Web.Api
{
    public class BaseApiController : ApiController, IController
    {
        public Context Context
        {
            get { return new Context(UserName); }
        }

        public Database Database
        {
            get { return Context.Database; }
        }

        public ILogger Logger
        {
            get { return Context.Logger; }
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