namespace Known.Blazor;

public class TablePage<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.Page == null && Model.Module == null)
            BuildFormList(builder);
        else
            BuildPageList(builder);
    }

    private void BuildFormList(RenderTreeBuilder builder)
    {
        builder.Div("kui-table form-list", () =>
        {
            if (!string.IsNullOrWhiteSpace(Model.Name) ||
                 Model.QueryColumns.Count > 0 ||
                 (Model.ShowToolbar && Model.Toolbar.HasItem))
            {
                builder.Component<KToolbar>()
                       .Set(c => c.ChildContent, b =>
                       {
                           b.Div(() =>
                           {
                               b.FormTitle(Model.Name);
                               if (Model.QueryColumns.Count > 0)
                                   UI.BuildQuery(b, Model);
                           });
                           if (Model.ShowToolbar && Model.Toolbar.HasItem)
                               UI.BuildToolbar(b, Model.Toolbar);
                       })
                       .Build();
            }
            UI.BuildTable(builder, Model);
        });
    }

    private void BuildPageList(RenderTreeBuilder builder)
    {
        if (Model.QueryColumns.Count > 0)
            builder.Div("kui-query", () => UI.BuildQuery(builder, Model));

        builder.Div("kui-table", () =>
        {
            if (Model.Tab.HasItem)
            {
                Model.Tab.Left = b => b.FormTitle(Model.PageName);
                if (Model.Toolbar.HasItem)
                    Model.Tab.Right = b => UI.BuildToolbar(b, Model.Toolbar);
                UI.BuildTabs(builder, Model.Tab);
            }
            else
            {
                builder.Component<KToolbar>()
                       .Set(c => c.ChildContent, b =>
                       {
                           b.Div(() =>
                           {
                               b.FormTitle(Model.PageName);
                               Model.ToolbarSlot?.Invoke(b);
                           });
                           if (Model.Toolbar.HasItem)
                               UI.BuildToolbar(b, Model.Toolbar);
                       })
                       .Build();
            }
            UI.BuildTable(builder, Model);
        });
    }
}