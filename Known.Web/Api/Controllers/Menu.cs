using System.Collections.Generic;
using Known.Platform;

namespace Known.Web.Api.Controllers
{
    class Menu
    {
        public string id { get; set; }
        public string pid { get; set; }
        public string code { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string url { get; set; }
        public bool expanded { get; set; }
        public List<Menu> children { get; set; }

        public static Menu GetMenu(Module module)
        {
            return new Menu
            {
                id = module.Id,
                pid = module.ParentId,
                code = module.Code,
                text = module.Name,
                iconCls = module.Icon,
                url = module.Url
            };
        }
    }
}