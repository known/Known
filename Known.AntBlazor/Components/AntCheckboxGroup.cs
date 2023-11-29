using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntCheckboxGroup : CheckboxGroup
{
    [Parameter] public string CodeType { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var codes = Cache.GetCodes(CodeType, false);
        Options = codes.ToCheckboxOptions();
    }
}