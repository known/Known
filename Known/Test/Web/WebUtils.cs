using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Known.Web
{
    /// <summary>
    /// Web效用类。
    /// </summary>
    public sealed class WebUtils
    {
        /// <summary>
        /// 获取主机协议和域名。
        /// </summary>
        /// <param name="uri">请求地址。</param>
        /// <returns>主机协议和域名。</returns>
        public static string GetHostName(Uri uri)
        {
            var name = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            var port = uri.Port;
            if (port != 80 && port != 443)
                name += string.Format(":{0}", port);

            return name;
        }

        /// <summary>
        /// 对 URL 字符串进行编码。
        /// </summary>
        /// <param name="url">要编码的 URL。</param>
        /// <returns>已编码的 URL。</returns>
        public static string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// 对已编码的 URL 字符串进行解码。
        /// </summary>
        /// <param name="url">已编码的 URL。</param>
        /// <returns>已解码的 URL。</returns>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// 对 HTML 字符串进行编码。
        /// </summary>
        /// <param name="html">要编码的 HTML。</param>
        /// <returns>已编码的 HTML。</returns>
        public static string HtmlEncode(string html)
        {
            return HttpUtility.HtmlEncode(html);
        }

        /// <summary>
        /// 对已编码的 HTML 字符串进行解码。
        /// </summary>
        /// <param name="html">已编码的 HTML。</param>
        /// <returns>已解码的 HTML。</returns>
        public static string HtmlDecode(string html)
        {
            return HttpUtility.HtmlDecode(html);
        }

        /// <summary>
        /// 对文本内容进行 HTML 格式化。
        /// </summary>
        /// <param name="text">要格式化的文本。</param>
        /// <returns>已格式化的文本。</returns>
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
        /// 获取应用程序绝对路径。
        /// </summary>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>绝对路径。</returns>
        public static string GetAbsoluteUrl(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        /// <summary>
        /// 添加请求参数片段。
        /// </summary>
        /// <param name="rawUrl">原始路径。</param>
        /// <param name="fragment">参数片段。</param>
        /// <returns>完整的请求地址。</returns>
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
        /// 获取操作系统名称。
        /// </summary>
        /// <param name="userAgent">浏览器用户代理信息。</param>
        /// <returns>操作系统名称。</returns>
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
        /// 确信虚拟文件路径存在，不存在则创建。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <returns>存在的文件路径。</returns>
        public static string EnsureFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var fileName = HttpContext.Current.Server.MapPath(path);
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return fileName;
        }

        /// <summary>
        /// 删除指定虚拟路径的文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        public static void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var fileName = HttpContext.Current.Server.MapPath(path);
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
