using Known.Extensions;
using System.Web;

namespace Known.Web.Extensions
{
    public static class HttpResponseExtension
    {
        public static void Navigate(this HttpResponse response, string url)
        {
            response.Redirect(url, true);
        }

        public static void WriteText(this HttpResponse response, string text)
        {
            response.Write(text);
            response.End();
        }

        public static void WriteText(this HttpResponse response, string format, params object[] args)
        {
            var text = string.Format(format, args);
            response.WriteText(text);
        }

        public static void WriteJson<T>(this HttpResponse response, T value)
        {
            var json = value.ToJson();
            response.WriteText(json);
        }

        public static void WriteError(this HttpResponse response, string message)
        {
            var text = string.Format("<div class=\"alert alert-danger\" role=\"alert\">{0}</div>", message);
            response.WriteText(text);
        }

        public static void WriteError(this HttpResponse response, string format, params object[] args)
        {
            var message = string.Format(format, args);
            response.WriteError(message);
        }
    }
}
