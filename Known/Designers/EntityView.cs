using Known.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityView : BaseView
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Tab.Items.Add(new ItemModel("脚本") { Content = BuildScript });
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
    }

    protected override void BuildCode(RenderTreeBuilder builder)
    {
    }

    private void BuildScript(RenderTreeBuilder builder)
    {
    }
}