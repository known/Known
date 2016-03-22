using Known.PLite;
using Known.Services;
using System;

namespace Known.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AppContext.RegisterService<IActionService, ActionService>();
            AppContext.RegisterService<ICodeService, CodeService>();
            AppContext.RegisterService<IFieldService, FieldService>();
            AppContext.RegisterService<IMenuService, MenuService>();
            AppContext.RegisterService<IPrototypeService, PrototypeService>();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}