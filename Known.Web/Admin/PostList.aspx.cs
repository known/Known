﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.Web.Admin
{
    public partial class PostList : AdminPage1
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("内容管理");
        }
    }
}