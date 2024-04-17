namespace Known.AntBlazor.Components;

public class AutoColumn<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Table { get; set; }
    [Parameter] public TItem Item { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Table == null || Table.Columns == null)
            return;

        var isDictionary = Table.IsDictionary;
        foreach (var item in Table.Columns)
        {
            if (!item.IsVisible)
                continue;

            if (isDictionary)
                BuildPropertyColumn(builder, item);
            else
                BuildColumn(builder, item);
        }

        if (Table.HasSum)
        {
            //TODO：总结行重复加载
            builder.Component<SummaryRow>()
                   .Set(c => c.ChildContent, BuildSummaryRow)
                   .Build();
        }
    }

    private void BuildPropertyColumn(RenderTreeBuilder builder, ColumnInfo item)
    {
        var data = Item as Dictionary<string, object>;
        data.TryGetValue(item.Id, out var value);
        builder.OpenComponent<PropertyColumn<Dictionary<string, object>, object>>(0);
        AddPropertyColumn(builder, (Dictionary<string, object> c) => c.ContainsKey(item.Id) ? c[item.Id] : "");
        AddAttributes(builder, item, value);
        builder.CloseComponent();
    }

    private void BuildColumn(RenderTreeBuilder builder, ColumnInfo item)
    {
        var propertyType = typeof(string);
        if (item.Property != null)
            propertyType = item.Property.PropertyType.UnderlyingSystemType;
        var columnType = typeof(Column<>).MakeGenericType(propertyType);
        var value = TypeHelper.GetPropertyValue(Item, item.Id);
        builder.OpenComponent(0, columnType);
        builder.AddAttribute(1, nameof(Column<TItem>.DataIndex), item.Id);
        AddAttributes(builder, item, value);
        builder.CloseComponent();
    }

    private void BuildSummaryRow(RenderTreeBuilder builder)
    {
        builder.Component<SummaryCell>()
               .Set(c => c.Class, "kui-table-check")
               .Set(c => c.Align, ColumnAlign.Center)
               .Set(c => c.ChildContent, b => b.Text(Language["IsSum"]))
               .Build();
        foreach (var item in Table.Columns)
        {
            if (item.IsSum)
            {
                object value = null;
                Table.Result?.Sums?.TryGetValue(item.Id, out value);
                BuildSummaryCell(builder, $"{value}");
            }
            else
            {
                BuildSummaryCell(builder, "");
            }
        }
        if (Table.HasAction)
            BuildSummaryCell(builder, "");
    }

    private void BuildSummaryCell(RenderTreeBuilder builder, string text)
    {
        builder.Component<SummaryCell>().Set(c => c.ChildContent, b => b.Text(text)).Build();
    }

    private static void AddPropertyColumn(RenderTreeBuilder builder, Expression<Func<Dictionary<string, object>, object>> property)
    {
        builder.AddAttribute(1, nameof(PropertyColumn<TItem, object>.Property), property);
    }

    private void AddAttributes(RenderTreeBuilder builder, ColumnInfo item, object value)
    {
        var title = Language?.GetString<TItem>(item);
        builder.AddAttribute(1, nameof(Column<TItem>.Ellipsis), true);
        builder.AddAttribute(1, nameof(Column<TItem>.Title), title);
        builder.AddAttribute(1, nameof(Column<TItem>.Hidden), !item.IsVisible);
        builder.AddAttribute(1, nameof(Column<TItem>.Sortable), item.IsSort);
        //TODO:固定列显示混乱问题
        //if (!string.IsNullOrWhiteSpace(item.Fixed))
        //    builder.AddAttribute(1, nameof(Column<TItem>.Fixed), item.Fixed);
        if (item.Width > 0)
            builder.AddAttribute(1, nameof(Column<TItem>.Width), $"{item.Width}");
        if (!string.IsNullOrWhiteSpace(item.Align))
            builder.AddAttribute(1, nameof(Column<TItem>.Align), GetColumnAlign(item.Align));
        if (!string.IsNullOrWhiteSpace(item.DefaultSort))
        {
            var sortName = item.DefaultSort == "Descend" ? "descend" : "ascend";
            builder.AddAttribute(1, nameof(Column<TItem>.DefaultSortOrder), SortDirection.Parse(sortName));
        }
        //builder.AddAttribute(1, nameof(Column<TItem>.Filterable), true);
        if (item.Type == FieldType.Date)
            builder.AddAttribute(1, nameof(Column<TItem>.Format), "yyyy-MM-dd");
        if (item.Type == FieldType.DateTime)
            builder.AddAttribute(1, nameof(Column<TItem>.Format), "yyyy-MM-dd HH:mm:ss");

        RenderFragment<TItem> template = null;
        Table.Templates?.TryGetValue(item.Id, out template);
        if (template != null)
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b => b.AddContent(1, template(Item))));
        }
        else if (value?.GetType() == typeof(bool))
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b =>
            {
                var isChecked = Utils.ConvertTo<bool>(value);
                b.Component<Switch>().Set(c => c.Checked, isChecked)
                                     .Set(c => c.Disabled, true)
                                     .Build();
            }));
        }
        else if (item.IsViewLink)
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b =>
            {
                b.Link($"{value}", this.Callback(() => Table.ViewForm(Item)));
            }));
        }
    }

    private static ColumnAlign GetColumnAlign(string align)
    {
        if (align == "center")
            return ColumnAlign.Center;
        else if (align == "right")
            return ColumnAlign.Right;
        return ColumnAlign.Left;
    }
}