using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Known.WebApi
{
    public class TrackHelper
    {
        public static string GetHeaders(HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            foreach (var item in request.Headers)
            {
                sb.Append($"{item.Key}=");
                item.Value.ToList().ForEach(v => sb.Append($"{v};"));
            }
            return sb.ToString();
        }

        public static string GetParameters(HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            switch (request.Method.Method)
            {
                case "GET":
                    request.GetQueryNameValuePairs().ToList()
                           .ForEach(e => sb.Append($"{e.Key}={e.Value};"));
                    break;
                case "POST":
                    var stream = request.Content.ReadAsStreamAsync().Result;
                    if (stream.CanSeek)
                        stream.Seek(0, SeekOrigin.Begin);
                    sb.Append(request.Content.ReadAsStringAsync().Result);
                    break;
            }
            return sb.ToString();
        }
    }
}