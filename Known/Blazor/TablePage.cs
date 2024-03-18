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
            builder.Div("kui-query", () => UI.BuildQuery(builder, Model));

        builder.Div("kui-table", () =>
        {
            if (Model.Tab.HasItem)
            {
                Model.Tab.Left = b => b.Component<KTitle>().Set(c => c.Text, Model.Module?.Name).Build();
                if (Model.Toolbar.HasItem)
                    Model.Tab.Right = b => UI.BuildToolbar(b, Model.Toolbar);
                builder.Div(() => UI.BuildTabs(builder, Model.Tab));
            }
            else
            {
                builder.Component<KToolbar>()
                       .Set(c => c.ChildContent, b =>
                       {
                           b.Div(() =>
                           {
                               b.Component<KTitle>().Set(c => c.Text, Model.Module?.Name).Build();
                               Model.ToolbarSlot?.Invoke(b);
                           });
                           if (Model.Toolbar.HasItem)
                               b.Div(() => UI.BuildToolbar(b, Model.Toolbar));
                       })
                       .Build();
            }
            UI.BuildTable(builder, Model);
        });
    }
}