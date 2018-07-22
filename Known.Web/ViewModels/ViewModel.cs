using Known.Platform;

namespace Known.Web.ViewModels
{
    public class ViewModel
    {
        public ViewModel(Module module)
        {
            Module = module;
        }

        public Module Module { get; }
    }
}