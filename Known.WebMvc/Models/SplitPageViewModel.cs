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
                LeftPartial = extension.LeftPartial;
                RightPartial = extension.RightPartial;
            }
        }

        public string LeftPartial { get; set; }
        public string RightPartial { get; set; }
    }

    internal class SplitPageInfo
    {
        public string LeftPartial { get; set; }
        public string RightPartial { get; set; }
    }
}