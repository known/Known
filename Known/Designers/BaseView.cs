using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class BaseView<TModel> : BaseComponent
{
    protected TabModel Tab { get; } = new();
    internal CodeService Service => new();

    [Parameter] public TModel Model { get; set; }

    internal virtual void SetModel(TModel model) => Model = model;
    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
    protected void BuildList<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new() => builder.Div("list-view", () => UI.BuildTable(builder, model));
    protected void BuildCode(RenderTreeBuilder builder, string code) => builder.Markup($"<pre class=\"kui-code\">{code}</pre>");
}