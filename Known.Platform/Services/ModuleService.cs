using Known.Extensions;
using Known.Platform.Repositories;

namespace Known.Platform.Services
{
    public class ModuleService : PlatformService
    {
        private IModuleRepository Repository { get; }

        public ModuleService(Context context, IModuleRepository repository) : base(context)
        {
            Repository = repository;
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

            var module = Repository.QueryById<Module>(id);
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

            module.Parent = Repository.QueryById<Module>(module.ParentId);
            SetParentModule(module.Parent);
        }
        #endregion
    }
}
