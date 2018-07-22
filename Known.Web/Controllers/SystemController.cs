using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class SystemController : AuthorizeController
    {
        public ActionResult ModuleView()
        {
            return View();
        }

        public ActionResult RoleView()
        {
            return View();
        }

        public ActionResult UserView()
        {
            return View();
        }
    }
}