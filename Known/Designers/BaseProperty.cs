using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseProperty<TModel> : BaseComponent where TModel : class, new()
{
    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal bool IsReadOnly => ReadOnly || Model == null;

    internal void SetModel(TModel model)
    {
        Model = model ?? new();
        StateChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model ??= new();
    }

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            builder.Div("property", () => BuildForm(builder));
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    protected virtual void BuildForm(RenderTreeBuilder builder) { }

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            builder.Label(Language[label]);
            template?.Invoke(builder);
        });
    }
}