namespace Known.Razor;

class TableHead<TItem> : BaseComponent
{
    private string curId = "";
    private string curOrder = "asc";

    [CascadingParameter] private KDataGrid<TItem> Grid { get; set; }
    [CascadingParameter] private Table<TItem> Table { get; set; }

    protected override void OnInitialized()
    {
        if (!string.IsNullOrWhiteSpace(Grid.OrderBy))
        {
            var orders = Grid.OrderBy.Split(' ');
            curId = orders[0];
            if (orders.Length > 1)
                curOrder = orders[1];
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.THead(attr =>
        {
            builder.Tr(attr =>
            {
                if (Grid.CustHead != null)
                {
                    Grid.CustHead.Invoke(builder);
                }
                else
                {
                    BuildHeadIndex(builder);
                    BuildHeadCheckBox(builder);

                    if (Grid.GridColumns != null && Grid.GridColumns.Count > 0)
                    {
                        foreach (var item in Grid.GridColumns)
                        {
                            if (!item.IsVisible)
                                continue;

                            BuildHeadColumn(builder, item);
                        }
                    }

                    BuildHeadAction(builder);
                }
            });
        });
    }

    private void BuildHeadIndex(RenderTreeBuilder builder)
    {
        builder.Th("fixed index", attr =>
        {
            if (Grid.ShowSetting)
                builder.Icon("fa fa-cog", "表格设置", Callback(Grid.ShowColumnSetting));
            else
                builder.Text("#");
        });
    }

    private void BuildHeadCheckBox(RenderTreeBuilder builder)
    {
        if (!Grid.ShowCheckBox)
            return;

        builder.Th("fixed check", attr =>
        {
            builder.Label("form-radio", attr =>
            {
                builder.Check(attr => attr.Checked(Grid.CheckAll).OnClick(Callback(() =>
                {
                    Grid.CheckAll = !Grid.CheckAll;
                    Grid.SelectedItems.Clear();
                    if (Grid.CheckAll && Grid.Data != null && Grid.Data.Count > 0)
                        Grid.SelectedItems.AddRange(Grid.Data);
                    Table.Changed();
                })));
            });
        });
    }

    private void BuildHeadColumn(RenderTreeBuilder builder, Column<TItem> item)
    {
        builder.Th(item.ClassName, attr =>
        {
            var canSort = Grid.IsSort && item.IsSort && !string.IsNullOrWhiteSpace(item.Id);
            if (item.Width > 0)
                attr.Style($"width:{item.Width}px;");
            if (canSort)
                attr.OnClick(Callback(() => OnSort(item)));
            builder.Text(item.Name);
            if (canSort)
                SetSortIcon(builder, item);
        });
    }

    private void BuildHeadAction(RenderTreeBuilder builder)
    {
        if (!Grid.HasAction())
            return;

        builder.Th("action", attr =>
        {
            if (Grid.ActionHead != null)
                Grid.ActionHead.Invoke(builder);
            else
                builder.Text("操作");
        });
    }

    private void OnSort(ColumnInfo item)
    {
        curId = item.Id;
        curOrder = curOrder == "asc" ? "desc" : "asc";
        Grid.OnSort(item, curOrder);
    }

    private void SetSortIcon(RenderTreeBuilder builder, ColumnInfo item)
    {
        if (curId == item.Id)
            builder.Icon($"sort fa fa-sort-{curOrder}");
        else
            builder.Icon("fa fa-sort");
    }
}