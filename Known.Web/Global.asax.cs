using System;
using System.Web.Http;

namespace Known.Web
{
    public class Global : ApiApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            GlobalConfiguration.Configure(ApiConfig.Register);
        }
    }
}