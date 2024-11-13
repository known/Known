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
    /// 取得表格选择列选择类型，默认单选。
    /// </summary>
    protected virtual TableSelectType SelectType => TableSelectType.Radio;

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

    /// <summary>
    /// 异步初始化表格选择器组件。
    /// </summary>
    /// <returns></returns>
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
        if (SelectType == TableSelectType.Radio)
            Table.OnRowDoubleClick = OnRowDoubleClick;
    }

    /// <summary>
    /// 呈现表格选择器内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildContent(RenderTreeBuilder builder) => builder.Table(Table);

    private Task OnRowDoubleClick(TItem item)
    {
        selectedItems = [];
        selectedItems.Add(item);
        OnValueChanged(selectedItems);
        return OnClose?.Invoke();
    }
}

/// <summary>
/// 系统用户弹窗选择器组件类。
/// </summary>
public class UserPicker : TablePicker<UserInfo>, ICustomField
{
    /// <summary>
    /// 异步初始化系统用户弹窗选择器组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Title = Language["Title.SelectUser"];
        Width = 800;
        AllowClear = true;
        Table.OnQuery = Platform.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }

    /// <summary>
    /// 选择器选择内容改变时触发的方法。
    /// </summary>
    /// <param name="items">选中的对象列表。</param>
    protected override void OnValueChanged(List<UserInfo> items)
    {
        var value = SelectType == TableSelectType.Checkbox
                  ? string.Join(",", items.Select(d => d.UserName))
                  : items?.FirstOrDefault()?.UserName;
        ValueChanged?.Invoke(value);
    }
}