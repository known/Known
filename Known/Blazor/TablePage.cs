using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class TablePage<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TablePageModel<TItem> Model { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-top", () =>
        {
            UI.BuildQuery(builder, Model);
            UI.BuildToolbar(builder, Model.Toolbar);
        });
        builder.Div("kui-table", () => UI.BuildTable(builder, Model));
    }
}