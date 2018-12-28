using Known.Extensions;
using Known.Platform;

namespace Known.WebMvc.Models
{
    public class SplitPageViewModel : ViewModel
    {
        public SplitPageViewModel(Module module) : base(module)
        {
            var extension = module.Extension.FromJson<SplitPageInfo>();
            if (extension != null)
            {
                LeftPartialName = extension.LeftPartialName;
                RightPartialName = extension.RightPartialName;
            }
        }

        public string LeftPartialName { get; set; }
        public string RightPartialName { get; set; }
    }

    internal class SplitPageInfo
    {
        public string LeftPartialName { get; set; }
        public string RightPartialName { get; set; }
    }
}