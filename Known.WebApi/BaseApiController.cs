using System.Web.Http;
using Known.Platform;

namespace Known.WebApi
{
    public class BaseApiController : ApiController
    {
        public string UserName
        {
            get { return User.Identity.Name; }
        }

        public bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        protected PlatformService PlatformService
        {
            get { return new PlatformService(); }
        }

        public T LoadService<T>() where T : ServiceBase
        {
            return Container.Resolve<T>(typeof(T).Name);
        }
    }
}