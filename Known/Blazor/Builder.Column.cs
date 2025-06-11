namespace Known.Blazor;

/// <summary>
/// 表格栏位建造者类，提供一系列表格栏位代码操作。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string id;
    private readonly ColumnInfo column;
    private readonly ColumnInfo allColumn;

    /// <summary>
    /// 取得表格组件模型对象。
    /// </summary>
    public TableModel<TItem> Table { get; }

    internal ColumnBuilder(ColumnInfo column, ColumnInfo allColumn = null, TableModel<TItem> table = null)
    {
        this.column = column;
        this.allColumn = allColumn;
        Table = table;

        if (column != null)
            id = column.Id;
    }

    /// <summary>
    /// 设置表格栏位是否可见。
    /// </summary>
    /// <param name="visible">是否可见。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Visible(bool visible)
    {
        if (column != null)
            column.IsVisible = visible;
        return this;
    }

    /// <summary>
    /// 设置表格栏位显示名称。
    /// </summary>
    /// <param name="name">显示名称。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Name(string name)
    {
        if (column != null)
            column.DisplayName = name;
        if (allColumn != null)
            allColumn.DisplayName = name;
        return this;
    }

    /// <summary>
    /// 设置表格栏位的提示信息。
    /// </summary>
    /// <param name="tooltip">提示信息。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Tooltip(string tooltip)
    {
        if (column != null)
            column.Tooltip = tooltip;
        return this;
    }

    /// <summary>
    /// 设置表格栏位关联的代码表类别。
    /// </summary>
    /// <param name="category">代码表类别。</param>
    /// <param name="isAll">下拉查询是否显示全部。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Category(string category, bool isAll = true)
    {
        if (column != null)
        {
            column.Category = category;
            column.IsQueryAll = isAll;
        }
        if (allColumn != null)
            allColumn.Category = category;
        return this;
    }
}