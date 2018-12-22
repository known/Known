using System.Web.Mvc;
using Known.Web.Filters;

namespace Known.Web
{
    public class FilterConfig
    {
        public static void Register(GlobalFilterCollection filters)
        {
            filters.Add(new ValidateInputAttribute(false));
            filters.Add(new TrackActionAttribute());
            filters.Add(new AntiForgeryAttribute());
        }
    }
}