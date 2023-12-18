using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseDesigner : BaseComponent
{
    [Parameter] public EntityInfo Entity { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        FieldChanged(Entity?.Fields?.FirstOrDefault());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Component<ColumnPanel>()
                       .Set(c => c.Entity, Entity)
                       .Set(c => c.FieldChanged, OnFieldChanged)
                       .Build();
            });
            BuildDesigner(builder);
        });
    }

    protected virtual void BuildDesigner(RenderTreeBuilder builder) { }
    protected virtual void FieldChanged(FieldInfo field) { }

    private Task OnFieldChanged(FieldInfo field)
    {
        FieldChanged(field);
        return Task.CompletedTask;
    }
}