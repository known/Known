using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty : BaseComponent
{
    public BaseProperty()
    {
        Column = new ColumnInfo();
    }

    public ColumnInfo Column { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {Column?.Id}"));
        BuildPropertyItem(builder, "属性", b => b.Span(Column?.Name));
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