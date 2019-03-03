using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Known.Core.Services;

namespace Known.WebMvc.Controllers.Develop
{
    public class DevDemoController : AuthorizeController
    {
        private DevDemoService Service
        {
            get { return Container.Resolve<DevDemoService>(); }
        }
    }
}