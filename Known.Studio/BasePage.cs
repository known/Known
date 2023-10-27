using Microsoft.JSInterop;

namespace Known.Studio;

class BasePage : BaseComponent
{
    [Inject] public IJSRuntime JS { get; set; }
}