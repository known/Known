using Known.Web;

namespace Known.WebMvc
{
    public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {
        public ApiClient Api { get; }
    }
}