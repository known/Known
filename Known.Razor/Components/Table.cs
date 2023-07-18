namespace Known.Razor.Components;

class Table<TItem> : BaseComponent
{
    [CascadingParameter] private DataGrid<TItem> Grid { get; set; }

    internal void Changed()
    {
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Grid.Data == null || Grid.Data.Count == 0)
        {
            BuildEmptyTable(builder);
            return;
        }

        builder.Table(attr =>
        {
            attr.Id(Grid.GridId);
            builder.Cascading(this, b => BuildDataTable(b));
        });
    }

    private void BuildEmptyTable(RenderTreeBuilder builder)
    {
        builder.Table(attr =>
        {
            attr.Id(Id);
            builder.Cascading(this, b => b.Component<TableHead<TItem>>().Build());
        });
        if (Grid.ShowEmpty)
            builder.Component<Empty>().Set(c => c.Text, Grid.EmptyText).Build();
    }

    private void BuildDataTable(RenderTreeBuilder builder)
    {
        builder.Component<TableHead<TItem>>().Build();
        builder.TBody(attr =>
        {
            var index = 0;
            foreach (TItem item in Grid.Data)
            {
                builder.Component<TableRow<TItem>>()
                       .Set(c => c.Index, index++)
                       .Set(c => c.Item, item)
                       .Build();
            }
        });
        builder.Component<TableFoot<TItem>>().Build();
    }
}