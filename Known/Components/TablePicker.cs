namespace Known.Components;

/// <summary>
/// 表格弹出选择器组件类。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TablePicker<TItem> : BasePicker<TItem> where TItem : class, new()
{
    private List<TItem> selectedItems;

    /// <summary>
    /// 取得表格组件配置模型对象。
    /// </summary>
    protected TableModel<TItem> Table { get; private set; }

    /// <summary>
    /// 取得或设置表格选择列选择类型，默认单选。
    /// </summary>
    protected TableSelectType SelectType { get; set; } = TableSelectType.Radio;

    /// <summary>
    /// 取得表格选中行绑定的数据对象列表。
    /// </summary>
    public override List<TItem> SelectedItems
    {
        get
        {
            if (selectedItems != null)
                return selectedItems;

            return Table.SelectedRows?.ToList();
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(this)
        {
            IsForm = true,
            AdvSearch = false,
            ShowPager = true,
            SelectType = SelectType
        };
        Table.OnResult = () =>
        {
            if (ItemExpression != null)
            {
                var rows = Table.DataSource?.Where(ItemExpression).ToList();
                if (rows != null && rows.Count > 0)
                    Table.SelectedRows = [.. rows];
            }
        };
        if (SelectType == TableSelectType.Radio)
            Table.OnRowDoubleClick = OnRowDoubleClick;
    }

    /// <inheritdoc />
    protected override void BuildContent(RenderTreeBuilder builder) => builder.Table(Table);

    private Task OnRowDoubleClick(TItem item)
    {
        selectedItems = [];
        selectedItems.Add(item);
        OnValueChanged(selectedItems);
        return OnClose?.Invoke();
    }
}