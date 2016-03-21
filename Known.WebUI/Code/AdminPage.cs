using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Known.SLite;

namespace Known.Web
{
    public class AdminPage<T> : AdminPage1 where T : Entity<T>, new()
    {
        protected IEnumerable<T> Models = new List<T>();
        protected T Model = new T();
        protected bool IsUpdate = false;

        protected override void HandleGetRequest()
        {
            var operate = Request.Get<string>("operate");
            var id = Request.Get<string>("id");
            if (operate == "update")
            {
                Model = Entity.FindById<T>(id);
                IsUpdate = true;
            }
            else if (operate == "delete")
            {
                Entity.Delete(id);
                ShowMessage("删除成功！");
            }
        }

        protected override void HandlePostRequest()
        {
            var errors = Model.Validate();
            if (errors == null)
            {
                errors = new List<string>();
            }
            AttachValidate(errors);

            if (errors.Count > 0)
            {
                ShowError(string.Join("<br/>", errors.ToArray()));
            }
            else
            {
                Model.Save();
                ShowMessage("保存成功！");
            }
        }

        protected virtual void AttachValidate(List<string> errors)
        {
        }
    }

    public class AdminPage1 : PageBase
    {
        public Admin.Admin MasterPage
        {
            get { return Page.Master as Known.Web.Admin.Admin; }
        }

        public string SitePath
        {
            get { return KConfig.SitePath; }
        }

        protected void SetPageTitle(string title)
        {
            MasterPage.Caption = title;

            if (title == "管理中心")
                title = "首页";
            Page.Title = title + " - 管理中心 - Powered by Known";
        }

        public void ShowError(string error)
        {
            var strMessage = "<div class=\"p_error\">";
            strMessage += error;
            strMessage += "</div>";
            MasterPage.Message = strMessage;
        }

        public void ShowMessage(string message)
        {
            var strMessage = "<div class=\"p_message\">";
            strMessage += message;
            strMessage += "</div>";
            MasterPage.Message = strMessage;
        }

        public string FormatUrl(string url)
        {
            return url.Replace("${siteurl}", KConfig.SiteUrl);
        }

        public string FormatEnum(Enum type)
        {
            var value = Convert.ToInt32(type);
            if (value == 0)
                return "管理员";
            return "普通用户";
        }

        public string GetErrorsHtml(List<string> errors)
        {
            return string.Format("<div class=\"errors container red\">{0}</div>", string.Join("<br/>", errors.ToArray()));
        }
    }
}