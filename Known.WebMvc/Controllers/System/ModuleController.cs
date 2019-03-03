using Known.Core.Services;

namespace Known.WebMvc.Controllers.System
{
    public class ModuleController : AuthorizeController
    {
        private ModuleService Service
        {
            get { return Container.Resolve<ModuleService>(); }
        }

        #region View
        #endregion

        #region Form
        #endregion
    }
}