using System.Text;
using Known.Web.Mvc;

namespace Known.Core.Web
{
    /// <summary>
    /// 文档控制器。
    /// </summary>
    public class DocController : Controller
    {
        /// <summary>
        /// 文档列表页面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var html = DocBuilder.GetList();
            return Content(html);
        }
    }

    class DocBuilder
    {
        public static string GetList()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h1>Api文档</h1>");

            var contollers = WebApp.GetControllers();
            if (contollers == null || contollers.Count == 0)
                return sb.ToString();

            foreach (var item in contollers.Values)
            {
                sb.AppendLine("<div>");
                sb.AppendLine($"<h2>{item.Name}Controller</h2>");
                foreach (var action in item.Actions)
                {
                    sb.AppendLine("<div>");
                    sb.AppendLine($"<h3>{action.Name}</h3>");
                    sb.AppendLine("</div>");
                }
                sb.AppendLine("</div>");
            }

            return sb.ToString();
        }
    }
}
