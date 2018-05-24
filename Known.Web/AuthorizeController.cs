using Known.Web.Filters;

namespace Known.Web
{
    /// <summary>
    /// 身份认证的控制器。
    /// </summary>
    [LoginAuthorize]
    public class AuthorizeController : BaseController
    {
    }
}