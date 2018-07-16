using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class DemoController : AuthorizeController
    {
        public ActionResult QueryUsers(string query, string isLoad)
        {
            return JsonResult(new { total = 10 });
        }
    }
}