using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Known.SLite;

namespace Known.Web
{
    public partial class Default : FrontPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SiteSetting.Instance.Enabled)
            {
                Response.Write("<h1>网站维护中......<h1>");
                Response.End();
            }

            var pageType = Request.Get<string>("pagetype", "default");
            var templatePath = Server.MapPath(string.Format("{0}/templates/{1}/", KConfig.SitePath, "default"));
            if (!Directory.Exists(templatePath))
            {
                templatePath = Server.MapPath(string.Format("{0}/templates/default/", KConfig.SitePath));
            }
            var te = new TemplateEngine(templatePath);
            SetFields(te, pageType);
            var html = te.BuildString(pageType + ".html");
            Response.Write(html.RemoveHtmlWhitespace());
        }

        private void SetFields(TemplateEngine te, string pageType)
        {
            te.Put(TagFields.IsDefault, pageType == "default" ? 1 : 0);
            te.Put(TagFields.PageType, pageType);
            te.Put(TagFields.SiteName, SiteSetting.Instance.SiteName ?? "");
            te.Put(TagFields.MetaKeywords, SiteSetting.Instance.MetaKeywords ?? "");
            te.Put(TagFields.MetaDescription, SiteSetting.Instance.MetaDescription ?? "");
            te.Put(TagFields.SitePath, KConfig.SitePath);
            te.Put(TagFields.SiteUrl, KConfig.SiteUrl);
            te.Put(TagFields.Errors, "");
            te.Put(TagFields.FooterHtml, FormatUrl(SiteSetting.Instance.FooterHtml ?? ""));
            var navLinks = Link.GetNavigationLinks();
            navLinks.ForEach(l => l.Url = FormatUrl(l.Url));
            te.Put(TagFields.NavLinks, navLinks);
            var links = Link.GetLinks();
            links.ForEach(l => l.Url = FormatUrl(l.Url));
            te.Put(TagFields.Links, links);

            switch (pageType)
            {
                case "login":
                    SetLoginFields(te);
                    break;
                case "register":
                    SetRegisterFields(te);
                    break;
                case "submitorder":
                    SetSubmitOrderFields(te);
                    break;
                case "myorder":
                    SetMyOrderFields(te);
                    break;
                case "about":
                    SetAboutFields(te);
                    break;
                case "blank":
                    SetBlankFields(te);
                    break;
                default:
                    break;
            }
        }

        private void SetLoginFields(TemplateEngine te)
        {
            te.Put(TagFields.PageTitle, "登录");
        }

        private void SetRegisterFields(TemplateEngine te)
        {
            te.Put(TagFields.PageTitle, "注册");

            if (Request.IsPost())
            {
                var member = new Member();
                member.RealName = Request.Get<string>("realName");
                member.Mobile = Request.Get<string>("userMobile");
                member.Password = Request.Get<string>("password").ToMd5();
                member.RegisterIP = Request.GetIPAddress();
                var errors = member.Validate();
                var password = Request.Get<string>("password");
                var password1 = Request.Get<string>("password1");
                if (string.IsNullOrEmpty(password))
                    errors.Add("设置密码不能为空！");
                if (string.IsNullOrEmpty(password1))
                    errors.Add("确认密码不能为空！");
                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(password1) && password != password1)
                    errors.Add("两次密码不一致！");

                if (errors.Count > 0)
                {
                    te.Put(TagFields.Errors, GetErrorsHtml(errors));
                }
                else
                {
                    member.Save();
                }
            }
        }

        private void SetSubmitOrderFields(TemplateEngine te)
        {
            te.Put(TagFields.PageTitle, "在线订车");
        }

        private void SetMyOrderFields(TemplateEngine te)
        {
            te.Put(TagFields.PageTitle, "我的订单");
        }

        private void SetAboutFields(TemplateEngine te)
        {
            var anchor = Request.Get<string>("anchor", "about");
            te.Put(TagFields.CurrentPage, anchor);
            switch (anchor)
            {
                case "contact":
                    te.Put(TagFields.PageTitle, "联系我们");
                    break;
                case "clause":
                    te.Put(TagFields.PageTitle, "服务条款");
                    break;
                default:
                    te.Put(TagFields.PageTitle, "公司简介");
                    break;
            }
        }

        private void SetBlankFields(TemplateEngine te)
        {
            var anchor = Request.Get<string>("anchor", "about");
            te.Put(TagFields.CurrentPage, anchor);
            switch (anchor)
            {
                case "agreement":
                    te.Put(TagFields.PageTitle, "服务协议");
                    break;
                default:
                    break;
            }
        }
    }
}