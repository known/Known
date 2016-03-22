using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Known.SLite;
using Known.Web.Extensions;
using Known.Extensions;

namespace Known.Web.Admin
{
    public partial class UserList : AdminPage<User>
    {
        protected string PasswordTips = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("用户管理");

            HandleGetRequest();
            PasswordTips = IsUpdate ? "(密码留空即不修改)" : "";

            if (Request.IsPost())
            {
                Model.UserType = Request.Get<UserType>("UserType");
                Model.UserName = Request.Get<string>("UserName");
                var password = Request.Get<string>("Password");
                if (!string.IsNullOrEmpty(password))
                    Model.Password = password.ToMd5();
                Model.Email = Request.Get<string>("Email");
                Model.DisplayOrder = Request.Get<int>("DisplayOrder");
                Model.Enabled = Request.Get<string>("Enabled") == "on";
                HandlePostRequest();
            }

            Models = Entity.FindAll<User>().OrderBy(l => l.DisplayOrder);
        }

        protected override void AttachValidate(List<string> errors)
        {
            var password = Request.Get<string>("Password");
            var password2 = Request.Get<string>("Password2");
            if (!IsUpdate && string.IsNullOrEmpty(password))
                errors.Add("密码不能为空！");
            if (!IsUpdate && string.IsNullOrEmpty(password2))
                errors.Add("确认密码不能为空！");
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(password2) && password != password2)
                errors.Add("两次密码不一致！");
        }
    }
}