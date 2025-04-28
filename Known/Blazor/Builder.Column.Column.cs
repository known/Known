namespace Known.Blazor;

public partial class ColumnBuilder<TItem>
{
    /// <summary>
    /// 设置表格栏位是否支持查询条件。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> QueryField(bool isQueryField)
    {
        if (column != null)
            column.IsQueryField = isQueryField;
        if (allColumn != null)
            allColumn.IsQueryField = isQueryField;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为查询字段。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Query()
    {
        if (column != null)
        {
            column.IsQuery = true;
            Table?.AddQueryColumn(column);
        }
        return this;
    }

    /// <summary>
    /// 设置表格栏位为查看连接。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> ViewLink()
    {
        if (column != null)
            column.IsViewLink = true;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为查看连接。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> ViewLink(Action<TItem> action)
    {
        if (column == null)
            return this;

        if (action == null)
        {
            column.IsViewLink = true;
            return this;
        }

        column.LinkAction = item => action.Invoke((TItem)item);
        return this;
    }

    /// <summary>
    /// 设置表格栏位为合并行字段。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> MergeRow()
    {
        if (column != null)
            column.IsMergeRow = true;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为合并列字段。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> MergeColumn()
    {
        if (column != null)
            column.IsMergeColumn = true;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为汇总字段。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Sum()
    {
        if (column != null)
            column.IsSum = true;
        return this;
    }

    /// <summary>
    /// 设置表格栏位宽度。
    /// </summary>
    /// <param name="width">宽度。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Width(int width)
    {
        if (column != null)
            column.Width = width;
        return this;
    }

    /// <summary>
    /// 设置表格栏位对齐方式。
    /// </summary>
    /// <param name="align">对齐方式（left/center/right）。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Align(string align)
    {
        if (column != null)
            column.Align = align;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为固定列。
    /// </summary>
    /// <param name="fixType">固定类型（left/right）。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Fixed(string fixType)
    {
        if (column != null)
            column.Fixed = fixType;
        return this;
    }

    /// <summary>
    /// 设置栏位文本超出宽度是否显示省略号，显示则文本不换行。
    /// </summary>
    /// <param name="elipsis">是否显示省略号。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Ellipsis(bool elipsis)
    {
        if (column != null)
            column.Ellipsis = elipsis;
        return this;
    }

    /// <summary>
    /// 设置表格栏位为排序字段。
    /// </summary>
    /// <param name="isSort">是否排序字段，默认是。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Sort(bool isSort = true)
    {
        if (column != null)
            column.IsSort = isSort;
        return this;
    }

    /// <summary>
    /// 设置表格栏位默认升序。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> DefaultAscend() => DefaultSort("asc");

    /// <summary>
    /// 设置表格栏位默认降序。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> DefaultDescend() => DefaultSort("desc");

    private ColumnBuilder<TItem> DefaultSort(string sort)
    {
        if (column != null)
            column.DefaultSort = sort;
        return this;
    }
}