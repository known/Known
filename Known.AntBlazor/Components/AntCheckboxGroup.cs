using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntCheckboxGroup : CheckboxGroup
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options = Codes.ToCheckboxOptions();
    }
}