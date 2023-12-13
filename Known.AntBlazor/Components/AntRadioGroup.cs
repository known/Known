using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

class AntRadioGroup : RadioGroup<string>
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        //Fixed单选按钮组切换不刷新问题
        OnChange = EventCallback.Factory.Create<string>(this, value => StateHasChanged());
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Options = Codes.ToRadioOptions();
    }
}