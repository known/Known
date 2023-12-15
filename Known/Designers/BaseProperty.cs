using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty : BaseComponent
{
    [Parameter] public ColumnInfo Column { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildPropertyItem(builder, "属性", b => b.Span(Column.Id));
    }

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            builder.Label(label);
            template?.Invoke(builder);
        });
    }
}