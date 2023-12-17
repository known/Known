using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityView : BaseView
{
    private TableModel<FieldInfo> table = new();

    [Parameter] public string Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Tab.Items.Add(new ItemModel("脚本") { Content = BuildScript });
        
        //table.OnQuery=
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        UI.BuildTable<FieldInfo>(builder, table);
    }

    protected override void BuildCode(RenderTreeBuilder builder)
    {
    }

    private void BuildScript(RenderTreeBuilder builder)
    {
    }
}