using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace Known.BootBlazor.Components;

class BootRadioList : RadioList<string>
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Items = Codes.ToSelectedItems();
    }
}