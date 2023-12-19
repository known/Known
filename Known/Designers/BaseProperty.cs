using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty<TModel> : BaseComponent where TModel : class, new()
{
    [Parameter] public TModel Model { get; set; } = new();
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal bool IsReadOnly => ReadOnly || Model == null;

    internal void SetModel(TModel model)
    {
        Model = model;
        StateChanged();
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