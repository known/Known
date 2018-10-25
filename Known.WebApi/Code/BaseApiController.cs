using System.Web.Http;

namespace Known.WebApi
{
    public class BaseApiController : ApiController, IController
    {
        public Context Context
        {
            get { return new Context(UserName); }
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