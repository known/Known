using Microsoft.JSInterop;

namespace Known.Studio;

public class BasePage : BaseComponent
{
    [Inject] public IJSRuntime JS { get; set; }
}