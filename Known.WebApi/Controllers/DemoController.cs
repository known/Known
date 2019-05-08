using Known.Web;

namespace Known.WebApi.Controllers
{
    public class DemoController : ApiControllerBase
    {
        public object Test(string param)
        {
            return ApiResult.Success($"Hi! {param}", param);
        }
    }
}