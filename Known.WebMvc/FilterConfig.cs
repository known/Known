using System.Web.Mvc;
using Known.WebMvc.Filters;

namespace Known.WebMvc
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