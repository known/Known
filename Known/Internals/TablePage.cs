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
        builder.Div("kui-table form-list", (Action)(() =>
        {
            if (!string.IsNullOrWhiteSpace(Model.Name) ||
                 Model.QueryColumns.Count > 0 ||
                 Model.ShowToolbar && Model.Toolbar.HasItem)
            {
                builder.Toolbar(() =>
                {
                    builder.Div(() =>
                    {
                        builder.FormTitle(Model.Name);
                        if (Model.QueryColumns.Count > 0)
                            builder.Query(Model);
                    });
                    if (Model.ShowToolbar && Model.Toolbar.HasItem)
                        builder.Toolbar(Model.Toolbar);
                });
            }
            builder.Component<KTable<TItem>>().Set(c => c.Model, Model).Build();
        }));
    }

    private void BuildPageList(RenderTreeBuilder builder)
    {
        if (Model.QueryColumns.Count > 0)
        {
            builder.Div("kui-table-page", () =>
            {
                builder.Div("kui-query", () => builder.Query(Model));
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
        builder.Div("kui-table", (Action)(() =>
        {
            if (Model.Tab.HasItem)
            {
                Model.Tab.Left = b => b.FormTitle(Model.PageName);
                if (Model.Toolbar.HasItem)
                    Model.Tab.Right = BuildRight;
                builder.Tabs(Model.Tab);
            }
            else
            {
                builder.Toolbar(() =>
                {
                    builder.Div(() =>
                    {
                        builder.FormTitle(Model.PageName);
                        if (Model.TopStatis != null)
                            builder.Component<ToolbarSlot<TItem>>().Set(c => c.Table, Model).Build();
                    });
                    builder.Div(() => BuildRight(builder));
                });
            }
            builder.Component<KTable<TItem>>().Set(c => c.Model, Model).Build();
        }));
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        if (Model.Toolbar.HasItem)
            builder.Toolbar(Model.Toolbar);
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