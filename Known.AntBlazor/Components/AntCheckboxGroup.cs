using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntCheckboxGroup : CheckboxGroup
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToCheckboxOptions(o =>
        {
            o.Checked = Value != null && Value.Contains(o.Value);
        });
    }
}