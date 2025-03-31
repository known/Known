namespace Known.Internals;

/// <summary>
/// 查询字段项目组件类。
/// </summary>
public partial class QueryItem
{
    private string itemType = "String";
    private string SelectStyle => IsInline ? "width:194px;" : "";

    /// <summary>
    /// 取得或设置字段标题。
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// 取得或设置是否在一行。
    /// </summary>
    [Parameter] public bool IsInline { get; set; }

    /// <summary>
    /// 取得或设置字段项目信息。
    /// </summary>
    [Parameter] public ColumnInfo Item { get; set; }

    /// <summary>
    /// 取得或设置查询条件字典对象。
    /// </summary>
    [Parameter] public Dictionary<string, QueryInfo> Data { get; set; }

    /// <summary>
    /// 取得或设置搜索操作委托。
    /// </summary>
    [Parameter] public Func<Task> OnSearch { get; set; }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        if (Item.Property != null)
            itemType = Item.Property.PropertyType.ToString();
        return base.OnInitAsync();
    }

    private async Task OnSelectChangedAsync(string id, string value)
    {
        if (value == Data[id].Value)
            return;

        Data[id].Value = value;
        await OnSearch?.Invoke();
    }
}