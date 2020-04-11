using Known.Core.Entities;

namespace Known.Core
{
    public class MenuInfo
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }

        internal object ToTree()
        {
            return new
            {
                id = Id,
                pid = ParentId,
                code = Code,
                title = Name,
                icon = Icon,
                url = Url
            };
        }

        internal static object ToTree(SysModule module)
        {
            return new
            {
                id = module.Id,
                pid = module.ParentId,
                code = module.Code,
                title = module.Name,
                icon = module.Icon,
                url = module.Url
            };
        }
    }
}
