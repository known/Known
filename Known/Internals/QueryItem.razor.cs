﻿namespace Known.Internals;

/// <summary>
/// 查询字段项目组件类。
/// </summary>
public partial class QueryItem
{
    private string itemType = "String";
    private string SelectStyle => IsInline ? "width:194px;" : (IsFilter ? "width:100px;" : "");
    private string InputStyle => IsFilter ? "width:100px;" : "";

    /// <summary>
    /// 取得或设置是否是列头过滤。
    /// </summary>
    [Parameter] public bool IsFilter { get; set; }

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
    [Parameter] public Func<List<QueryInfo>, Task> OnSearch { get; set; }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        if (Item.Property != null)
            itemType = Item.Property.PropertyType.ToString();

        if (IsFilter && !Data.ContainsKey(Item.Id))
            Data[Item.Id] = new QueryInfo(Item);

        return base.OnInitAsync();
    }

    private async Task OnSelectChangedAsync(string id, string[] values)
    {
        var value = string.Join(",", values ?? []);
        if (value == Data[id].Value)
            return;

        Data[id].Type = QueryType.In;
        Data[id].Value = value;
        await SearchDataAsync();
    }

    private async Task OnSelectChangedAsync(string id, string value)
    {
        if (value == Data[id].Value)
            return;

        Data[id].Value = value;
        await SearchDataAsync();
    }

    private async Task SearchDataAsync()
    {
        var query = Data.Select(d => d.Value).ToList();
        await OnSearch?.Invoke(query);
    }
}