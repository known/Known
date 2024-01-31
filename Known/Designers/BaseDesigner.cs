using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseDesigner<TModel> : BaseComponent
{
    [CascadingParameter] internal SysModuleForm Form { get; set; }

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }
}

class BaseViewDesigner<TModel> : BaseDesigner<TModel>
{
    internal BaseView<TModel> view;
    internal List<FieldInfo> Fields { get; set; } = [];

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            builder.Cascading(this, BuildTree);
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    private void BuildTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Component<ColumnPanel<TModel>>()
                       .Set(c => c.ReadOnly, ReadOnly)
                       .Set(c => c.OnFieldCheck, OnFieldCheck)
                       .Set(c => c.OnFieldClick, OnFieldClick)
                       .Build();
            });
            builder.Div("panel-view", () => BuildView(builder));
            builder.Div("panel-property", () => BuildProperty(builder));
        });
    }

    protected virtual void BuildView(RenderTreeBuilder builder) { }
    protected virtual void BuildProperty(RenderTreeBuilder builder) { }
    protected virtual void OnFieldCheck(FieldInfo field) { }
    protected virtual void OnFieldClick(FieldInfo field) { }
}