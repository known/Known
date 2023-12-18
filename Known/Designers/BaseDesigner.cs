using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseDesigner : BaseComponent
{
    internal BaseView view;
    internal BaseProperty property;

    [Parameter] public EntityInfo Entity { get; set; }

    internal List<FieldInfo> Fields { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        OnFieldClick(Entity?.Fields?.FirstOrDefault());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Component<ColumnPanel>()
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

    private void OnFieldCheck() { }

    private void OnFieldClick(FieldInfo field)
    {
        property?.SetField(field);
    }
}