using System.Text.RegularExpressions;
using System.Web;

namespace Known.Web
{
    /// <summary>
    /// XSS攻击检查者。
    /// </summary>
    public class XSSChecker
    {
        private const string StrRegex = @"<[^>]+?style=[\w]+?:expression\(|\b(alert|confirm|prompt)\b|^\+/v(8|9)|<[^>]*?=[^>]*?&#[^>]*?>|\b(and|or)\b.{1,6}?(=|>|<|\bin\b|\blike\b)|/\*.+?\*/|<\s*script\b|<\s*img\b|\bEXEC\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\s+(TABLE|DATABASE)";
        private readonly HttpRequest request;

        /// <summary>
        /// 初始化一个XSS攻击检查者对象。
        /// </summary>
        /// <param name="request">Http请求对象。</param>
        public XSSChecker(HttpRequest request)
        {
            this.request = request;
        }

        /// <summary>
        /// 检查攻击请求。
        /// </summary>
        /// <param name="error">错误信息。</param>
        /// <returns>是否通过检查。</returns>
        public bool Check(out string error)
        {
            error = "您提交的数据有恶意字符！";
            if (request.Cookies != null)
            {
                if (CheckCookieData())
                    return false;
            }
            if (request.UrlReferrer != null)
            {
                if (CheckUrlReferer())
                    return false;
            }
            if (request.RequestType.ToUpper() == "POST")
            {
                if (CheckPostData())
                    return false;
            }
            if (request.RequestType.ToUpper() == "GET")
            {
                if (CheckGetData())
                    return false;
            }

            error = string.Empty;
            return true;
        }

        private bool CheckPostData()
        {
            for (int i = 0; i < request.Form.Count; i++)
            {
                try
                {
                    if (CheckData(request.Form[i].ToString()))
                        return true;
                }
                catch
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckGetData()
        {
            for (int i = 0; i < request.QueryString.Count; i++)
            {
                try
                {
                    if (CheckData(request.QueryString[i].ToString()))
                        return true;
                }
                catch
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCookieData()
        {
            for (int i = 0; i < request.Cookies.Count; i++)
            {
                try
                {
                    if (CheckData(request.Cookies[i].Value.ToLower()))
                        return true;
                }
                catch
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckUrlReferer()
        {
            try
            {
                return CheckData(request.UrlReferrer.ToString());
            }
            catch
            {
                return true;
            }
        }

        private static bool CheckData(string inputData)
        {
            return Regex.IsMatch(inputData, StrRegex, RegexOptions.IgnoreCase);
        }
    }
}
