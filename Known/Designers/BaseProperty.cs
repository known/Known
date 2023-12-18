using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty : BaseComponent
{
    [Parameter] public FieldInfo Field { get; set; }

    internal void SetField(FieldInfo field)
    {
        Field = field;
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {Field?.Id}"));
        BuildPropertyItem(builder, "属性", b => b.Span(Field?.Name));
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