namespace Known.Internals;

class TablePage<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.IsForm)
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
                 Model.ShowToolbar && Model.Toolbar.HasItem)
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
        {
            builder.Div("kui-table-page", () =>
            {
                builder.Div("kui-query", () => UI.BuildQuery(builder, Model));
                BuildTable(builder);
            });
        }
        else
        {
            BuildTable(builder);
        }
    }

    private void BuildTable(RenderTreeBuilder builder)
    {
        builder.Div("kui-table", () =>
        {
            if (Model.Tab.HasItem)
            {
                Model.Tab.Left = b => b.FormTitle(Model.PageName);
                if (Model.Toolbar.HasItem)
                    Model.Tab.Right = BuildRight;
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
                               if (Model.TopStatis != null)
                                   b.Component<ToolbarSlot<TItem>>().Set(c => c.Table, Model).Build();
                           });
                           b.Div(() => BuildRight(b));
                       })
                       .Build();
            }
            UI.BuildTable(builder, Model);
        });
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        if (Model.Toolbar.HasItem)
            UI.BuildToolbar(builder, Model.Toolbar);
        if (Model.ShowSetting)
            builder.Component<TableSetting<TItem>>().Set(c => c.Table, Model).Build();
    }
}

class ToolbarSlot<TItem> : BaseComponent where TItem : class, new()
{
    private PagingResult<TItem> result;

    [Parameter] public TableModel<TItem> Table { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.OnRefreshStatis = OnRefreshStatis;
    }

    protected override void BuildRender(RenderTreeBuilder builder) => builder.Fragment(Table.TopStatis, result);

    private Task OnRefreshStatis(PagingResult<TItem> result)
    {
        this.result = result;
        return StateChangedAsync();
    }
}