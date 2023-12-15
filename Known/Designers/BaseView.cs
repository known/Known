using Known.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseView : BaseComponent
{
    protected TabModel Tab { get; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Tab.Items.Add(new ItemModel("视图") { Content = BuildView });
        Tab.Items.Add(new ItemModel("代码") { Content = BuildCode });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
    protected virtual void BuildView(RenderTreeBuilder builder) { }
    protected virtual void BuildCode(RenderTreeBuilder builder) { }
}