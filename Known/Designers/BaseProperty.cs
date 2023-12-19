using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty<TModel> : BaseComponent where TModel : class, new()
{
    private FieldInfo _field;
    [Parameter] public TModel Model { get; set; } = new();

    internal void SetField(FieldInfo field)
    {
        _field = field;
        Model = GetModel(field);
        StateChanged();
    }

    protected virtual TModel GetModel(FieldInfo field) => new();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {_field?.Id}"));
        BuildPropertyItem(builder, "属性", b => b.Span(_field?.Name));
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