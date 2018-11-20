using Known.Extensions;

namespace Known.Platform.Business
{
    public class ModuleBusiness : PlatformBusiness
    {
        public ModuleBusiness(Context context) : base(context)
        {
        }

        #region GetModule
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

            var module = Database.QueryById<Module>(id);
            if (module != null)
            {
                SetParentModule(module);
            }
            return module;
        }

        private void SetParentModule(Module module)
        {
            if (module.ParentId == "0")
                return;

            module.Parent = Database.QueryById<Module>(module.ParentId);
            SetParentModule(module.Parent);
        }
        #endregion
    }
}
