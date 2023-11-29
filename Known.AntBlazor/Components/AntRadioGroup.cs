using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntRadioGroup : RadioGroup<string>
{
    [Parameter] public string CodeType { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var codes = Cache.GetCodes(CodeType, false);
        Options = codes.ToRadioOptions();
        //Fixed单选按钮组切换不刷新问题
        OnChange = EventCallback.Factory.Create<string>(this, value => StateHasChanged());
    }
}