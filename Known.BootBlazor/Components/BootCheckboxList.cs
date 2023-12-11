using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace Known.BootBlazor.Components;

class BootCheckboxList : CheckboxList<string>
{
    [Parameter] public List<CodeInfo> Codes { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Items = Codes.ToSelectedItems(o =>
        {
            o.Active = !string.IsNullOrWhiteSpace(Value) && Value.Contains(o.Value);
        });
    }
}