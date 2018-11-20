using Known.Extensions;

namespace Known.Platform.Business
{
    public class ModuleBusiness : PlatformBusiness
    {
        public ModuleBusiness(Context context) : base(context)
        {
        }

        public Module GetModule(string id)
        {
            if (id == "devTool")
            {
                return new Module
                {
                    Id = "devTool",
                    Code = "DevTool",
                    Name = "开发工具",
                    ViewType = ViewType.SplitPageView,
                    Extension = new { LeftPartialName = "DevTool/LeftMenu" }.ToJson()
                };
            }

            return Database.QueryById<Module>(id);
        }
    }
}
