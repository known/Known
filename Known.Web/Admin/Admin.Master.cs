using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.Web.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        public string Caption { get; set; }
        public string Message { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}