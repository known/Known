using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Known.Extensions;

namespace Known.WebApi.Controllers
{
    public class UserController : BaseController
    {
        [HttpPost]
        public string SignIn(string account, string password)
        {
            return new { account, password }.ToJson();
        }
    }
}