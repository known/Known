using System.Web.Http;
using Known.Platform;

namespace Known.WebApi
{
    public class ApiControllerBase : ApiController
    {
        protected Context Context
        {
            get { return Context.Create(UserName); }
        }

        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        protected PlatformService PlatformService
        {
            get { return new PlatformService(); }
        }

        protected T LoadService<T>() where T : ServiceBase
        {
            return Container.Resolve<T>();
        }
    }
}