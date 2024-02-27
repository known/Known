using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class TablePage<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.QueryColumns.Count > 0)
            builder.Div("kui-table-query", () => UI.BuildQuery(builder, Model));

        builder.Div("kui-table", () =>
        {
            if (Model.Toolbar.HasItem)
            {
                builder.Div("kui-table-toolbar", () =>
                {
                    builder.Div("kui-table-left", () => builder.Div("kui-table-title", Model.Module?.Name));
                    builder.Div("kui-table-right", () => UI.BuildToolbar(builder, Model.Toolbar));
                });
            }
            UI.BuildTable(builder, Model);
        });
    }
}