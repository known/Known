using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    [Inject] internal ICodeService Service { get; set; }

    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal virtual void SetModel(TModel model) => Model = model;
    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
    protected void BuildList<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new() => builder.Div("list-view", () => UI.BuildTable(builder, model));
    protected void BuildCode(RenderTreeBuilder builder, string code) => builder.Markup($"<pre class=\"kui-code\">{code}</pre>");

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            builder.Label(label);
            template?.Invoke(builder);
        });
    }
}