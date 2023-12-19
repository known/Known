using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseDesigner<TModel> : BaseComponent
{
    internal BaseView<TModel> view;

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal List<FieldInfo> Fields { get; set; } = [];

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Component<ColumnPanel>()
                       .Set(c => c.ReadOnly, ReadOnly)
                       .Set(c => c.Entity, Entity)
                       .Set(c => c.Fields, Fields)
                       .Set(c => c.OnFieldCheck, OnFieldCheck)
                       .Set(c => c.OnFieldClick, OnFieldClick)
                       .Build();
            });
            BuildDesigner(builder);
        });
    }

    protected virtual void BuildDesigner(RenderTreeBuilder builder) { }
    protected virtual void OnFieldCheck() { }
    protected virtual void OnFieldClick(FieldInfo field) { }
}