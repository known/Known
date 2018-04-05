using System;
using System.Collections.Generic;
using System.Web;

namespace Known.Web
{
    /// <summary>
    /// Web效用类。
    /// </summary>
    public sealed class WebUtils
    {
        /// <summary>
        /// 根据Uri获取远程主机协议及域名。
        /// </summary>
        /// <param name="uri">URI。</param>
        /// <returns>远程主机协议及域名。</returns>
        public static string GetHostName(Uri uri)
        {
            var name = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            var port = uri.Port;
            if (port != 80 && port != 443)
                name += string.Format(":{0}", port);

            return name;
        }

        /// <summary>
        /// 格式化HTML内容。
        /// </summary>
        /// <param name="text">HTML内容。</param>
        /// <returns>格式化后HTML内容。</returns>
        public static string FormatHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            //text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            text = text.Replace("\r\n", "<br/>");
            text = text.Replace("\r", "<br/>");
            text = text.Replace("\n", "<br/>");
            return text;
        }

        /// <summary>
        /// 添加URL参数片段。
        /// </summary>
        /// <param name="rawUrl">原始URL。</param>
        /// <param name="fragment">参数片段。</param>
        /// <returns>添加后的完整URL。</returns>
        public static string AddUrlFragment(string rawUrl, string fragment)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(fragment))
                return rawUrl;

            if (!rawUrl.Contains("?"))
                return rawUrl + "?" + fragment;

            var fragments = fragment.Split('=');
            if (!rawUrl.Contains(fragments[0] + "="))
                return rawUrl + "&" + fragment;

            var rtnUrls = rawUrl.Split('?');
            var rtnFragments = new List<string>();
            var rawFragments = rtnUrls[1].Split('&');
            foreach (var item in rawFragments)
            {
                if (item.StartsWith(fragments[0] + "="))
                    rtnFragments.Add(fragment);
                else
                    rtnFragments.Add(item);
            }
            return rtnUrls[0] + "?" + string.Join("&", rtnFragments);
        }

        /// <summary>
        /// 根据浏览器代理信息获取客户端操作系统名。
        /// </summary>
        /// <param name="userAgent">浏览器代理信息。</param>
        /// <returns>客户端操作系统名。</returns>
        public static string GetOSName(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return string.Empty;

            var osName = string.Empty;
            if (userAgent.Contains("NT 10.0"))
            {
                osName = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.3"))
            {
                osName = "Windows 8.1";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                osName = "Windows 8";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osName = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osName = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                if (userAgent.Contains("64"))
                    osName = "Windows XP";
                else
                    osName = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osName = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osName = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osName = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osName = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osName = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osName = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osName = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osName = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osName = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osName = "SunOS";
            }
            return osName;
        }

        /// <summary>
        /// 确信服务端文件路径已经创建。
        /// </summary>
        /// <param name="path">文件虚拟路径。</param>
        /// <returns>服务端文件路径。</returns>
        public static string EnsureFile(string path)
        {
            var fileName = HttpContext.Current.Server.MapPath(path);
            return Utils.EnsureFile(fileName);
        }

        /// <summary>
        /// 删除服务端文件。
        /// </summary>
        /// <param name="path">文件虚拟路径。</param>
        public static void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var fileName = HttpContext.Current.Server.MapPath(path);
            Utils.DeleteFile(fileName);
        }
    }
}
