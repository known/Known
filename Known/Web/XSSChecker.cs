using System.Text.RegularExpressions;
using System.Web;

namespace Known.Web
{
    public class XSSChecker
    {
        private const string StrRegex = @"<[^>]+?style=[\w]+?:expression\(|\b(alert|confirm|prompt)\b|^\+/v(8|9)|<[^>]*?=[^>]*?&#[^>]*?>|\b(and|or)\b.{1,6}?(=|>|<|\bin\b|\blike\b)|/\*.+?\*/|<\s*script\b|<\s*img\b|\bEXEC\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\s+(TABLE|DATABASE)";
        private readonly HttpRequest request;

        public XSSChecker(HttpRequest request)
        {
            this.request = request;
        }

        public bool CheckPostData()
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

        public bool CheckGetData()
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

        public bool CheckCookieData()
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

        public bool CheckUrlReferer()
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
