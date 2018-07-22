using Known.Platform;

namespace Known.Web.ViewModels
{
    public class SplitPageViewModel : ViewModel
    {
        public SplitPageViewModel(Module module) : base(module)
        {
        }

        public string RightPartialName { get; set; }
    }
}