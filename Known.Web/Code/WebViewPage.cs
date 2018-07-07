using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Known.Web
{
    public abstract class WebViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {
        public ApiClient Api { get; }
    }
}