using System.Web.Http;

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

        public T LoadService<T>() where T : ServiceBase
        {
            return Container.Resolve<T>(typeof(T).Name);
        }
    }
}