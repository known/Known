using System.Web.Http;

namespace Known.Web.Api.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpPost]
        public string SignIn(string json)
        {
            return json;
        }
    }
}