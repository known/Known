using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Known.Web
{
    public class UrlRewriter : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.Application_BeginRequest);
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            foreach (var rule in GetRuleList())
            {
                var lookFor = "^" + ResolveUrl(app.Request.ApplicationPath, rule.LookFor) + "$";
                var re = new Regex(lookFor, RegexOptions.IgnoreCase);
                if (IsHttpUrl(rule.LookFor))
                {
                    if (re.IsMatch(app.Request.Url.AbsoluteUri))
                    {
                        var sendTo = ResolveUrl(app.Context.Request.ApplicationPath, re.Replace(app.Request.Url.AbsoluteUri, rule.SendTo));
                        RewritePath(app.Context, sendTo);
                        break;
                    }
                }
                else
                {
                    if (re.IsMatch(app.Request.Path))
                    {
                        var sendTo = ResolveUrl(app.Context.Request.ApplicationPath, re.Replace(app.Request.Path, rule.SendTo));
                        RewritePath(app.Context, sendTo);
                        break;
                    }
                }
            }
        }

        private List<RewriterRule> GetRuleList()
        {
            var cacheKey = KConfig.KeyPrefix + "rewriterulelist";
            var ruleList = (List<RewriterRule>)HttpContext.Current.Cache.Get(cacheKey);
            if (ruleList == null)
            {
                ruleList = new List<RewriterRule>();
                var urlFilePath = HttpContext.Current.Server.MapPath(KConfig.RewriteConfig);
                var xml = new System.Xml.XmlDocument();
                xml.Load(urlFilePath);

                var root = xml.SelectSingleNode("rewrite");
                foreach (XmlNode n in root.ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment && n.Name.ToLower() == "item")
                    {
                        var rule = new RewriterRule();
                        rule.LookFor = KConfig.SitePath + n.Attributes["lookfor"].Value;
                        rule.SendTo = KConfig.SitePath + n.Attributes["sendto"].Value;
                        ruleList.Add(rule);
                    }
                }
                HttpContext.Current.Cache.Insert(cacheKey, ruleList, new CacheDependency(urlFilePath));
            }
            return ruleList;
        }

        private string ResolveUrl(string appPath, string url)
        {
            //   return url;
            if (url.Length == 0 || url[0] != '~')
                return url;		// there is no ~ in the first character position, just return the url
            else
            {
                if (url.Length == 1)
                    return appPath;  // there is just the ~ in the URL, return the appPath
                if (url[1] == '/' || url[1] == '\\')
                {
                    // url looks like ~/ or ~\
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(2);
                    else
                        return "/" + url.Substring(2);
                }
                else
                {
                    // url looks like ~something
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(1);
                    else
                        return appPath + url.Substring(1);
                }
            }
        }

        private bool IsHttpUrl(string url)
        {
            return url.IndexOf("http://") != -1;
        }

        private void RewritePath(HttpContext context, string sendToUrl)
        {
            if (context.Request.QueryString.Count > 0)
            {
                if (sendToUrl.IndexOf('?') != -1)
                {
                    sendToUrl += "&" + context.Request.QueryString.ToString();
                }
                else
                {
                    sendToUrl += "?" + context.Request.QueryString.ToString();
                }
            }
            var queryString = string.Empty;
            var sendToUrlLessQString = sendToUrl;
            if (sendToUrl.IndexOf('?') > 0)
            {
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
                queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
            }
            //context.RewritePath(sendToUrlLessQString +"?"+ queryString);
            context.RewritePath(sendToUrlLessQString, string.Empty, queryString);
        }
    }

    public class RewriterRule
    {
        public string LookFor { get; set; }
        public string SendTo { get; set; }
    }
}