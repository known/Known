namespace Known.Razor.Components;

class TableRow<TItem> : BaseComponent
{
    [CascadingParameter] private DataGrid<TItem> Grid { get; set; }
    [CascadingParameter] private Table<TItem> Table { get; set; }

    [Parameter] public int Index { get; set; }
    [Parameter] public TItem Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var rowNo = Index + 1;
        var css = CssBuilder.Default("")
                            .AddClass("even", rowNo % 2 == 0)
                            .AddClass("active", Grid.CurRow == Index)
                            .AddClass("selected", Grid.SelectedItems.Contains(Item))
                            .Build();
        builder.Tr(css, attr =>
        {
            attr.Title(Grid.RowTitle);
            var context = new FieldContext { Model = Item };
            attr.OnClick(Callback(() =>
            {
                Grid.CurRow = Grid.Data.IndexOf(Item);
                Grid.OnRowClick(rowNo, Item);
                Table.Changed();
            }));

            attr.OnDoubleClick(Callback(() => Grid.OnRowDoubleClick(rowNo, Item)));
            BuildRowIndex(builder, rowNo);
            BuildRowCheckBox(builder, rowNo, Item);

            if (Grid.IsEdit)
            {
                builder.Component((Action<AttributeBuilder<CascadingValue<FieldContext>>>)(attr =>
                {
                    attr.Set(c => c.IsFixed, false)
                        .Set(c => c.Value, context)
                        .Set(c => c.ChildContent, BuildTree<TItem>(BuildRowContent).Invoke(Item));
                }));
            }
            else
            {
                BuildRowContent(builder, Item);
            }

            BuildRowAction(builder, rowNo, Item);
        });
    }

    private static void BuildRowIndex(RenderTreeBuilder builder, int index)
    {
        builder.Td("fixed index", attr => builder.Text($"{index}"));
    }

    private void BuildRowCheckBox(RenderTreeBuilder builder, int index, TItem item)
    {
        if (!Grid.ShowCheckBox)
            return;

        builder.Td("fixed check", attr =>
        {
            builder.Label("form-radio", attr =>
            {
                builder.Check(attr =>
                {
                    attr.Checked(Grid.SelectedItems.Contains(item)).OnClick(Callback(e =>
                    {
                        if (Grid.SelectedItems.Contains(item))
                            Grid.SelectedItems.Remove(item);
                        else
                            Grid.SelectedItems.Add(item);
                        Grid.CheckAll = Grid.SelectedItems.Count == Grid.TotalCount ||
                                        Grid.SelectedItems.Count == Grid.PageSize;
                    }));
                });
                builder.Span("");
            });
        });
    }

    private void BuildRowContent(RenderTreeBuilder builder, TItem item)
    {
        if (Grid.GridColumns == null || Grid.GridColumns.Count == 0)
            return;

        var data = Utils.MapTo<Dictionary<string, object>>(item);
        foreach (var column in Grid.GridColumns)
        {
            if (!column.IsVisible)
                continue;

            builder.Td(column.ClassName, attr =>
            {
                var value = data != null && data.ContainsKey(column.Id)
                          ? data[column.Id]
                          : null;
                column.BuildCell(builder, item, value, Grid.ReadOnly);
            });
        }
    }

    private void BuildRowAction(RenderTreeBuilder builder, int index, TItem item)
    {
        if (!Grid.HasAction())
            return;

        builder.Td("action", attr =>
        {
            BuildRowAction(builder, item, Grid.Actions);
            //var count = 0;
            //var actions = new List<ButtonInfo>();
            //var others = new List<ButtonInfo>();
            //foreach (var action in Grid.Actions)
            //{
            //    if (!Grid.CheckAction(action, item))
            //        continue;

            //    if (count++ < 2 || Grid.Actions.Count == 3)
            //        actions.Add(action);
            //    else
            //        others.Add(action);
            //}
            //BuildRowAction(builder, item, actions);
            //BuildRowMoreAction(builder, index, item, others);
        });
    }

    private void BuildRowAction(RenderTreeBuilder builder, TItem item, List<ButtonInfo> actions)
    {
        foreach (var action in actions)
        {
            if (!Grid.CheckAction(action, item))
                continue;

            var style = action.Style?.Replace("bg-", "");
            builder.Icon($"{action.Icon} {style}", action.Name, Callback(() => Grid.OnAction(action, new object[] { item })));
            //builder.Link(action.Name, Callback(() => Table.OnRowAction(action, item)), action.Style);
        }
    }

    //private void BuildRowMoreAction(RenderTreeBuilder builder, int index, TItem item, List<ButtonInfo> others)
    //{
    //    if (others.Count == 0)
    //        return;

    //    var items = others.Select(i => new DropdownItem(i, () => Grid.OnAction(i, new object[] { item }))).ToList();
    //    builder.Component<Dropdown>()
    //           .Set(c => c.Style, "link primary")
    //           .Set(c => c.Title, "更多")
    //           .Set(c => c.Items, items)
    //           .Build();
    //}
}