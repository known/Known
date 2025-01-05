namespace Known.Blazor;

/// <summary>
/// 表格栏位建造者类，提供一系列表格栏位代码操作。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string id;
    private readonly ColumnInfo column;

    /// <summary>
    /// 取得表格组件模型对象。
    /// </summary>
    public TableModel<TItem> Table { get; }

    internal ColumnBuilder(ColumnInfo column, TableModel<TItem> table = null)
    {
        this.column = column;
        Table = table;

        if (column != null)
            id = column.Id;
    }

    /// <summary>
    /// 设置表格栏位呈现模板。
    /// </summary>
    /// <param name="template">呈现模板。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Template(RenderFragment template)
    {
        if (column != null)
            column.Template = template;
        return this;
    }

    /// <summary>
    /// 设置表格栏位呈现模板。
    /// </summary>
    /// <param name="template">呈现模板。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        if (string.IsNullOrWhiteSpace(id))
            return this;

        if (Table != null)
            Table.Templates[id] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
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
    /// 设置表格栏位是否只读。
    /// </summary>
    /// <param name="readOnly">是否只读。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> ReadOnly(bool readOnly)
    {
        if (column != null)
            column.ReadOnly = readOnly;
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
    /// 设置表格栏位显示名称。
    /// </summary>
    /// <param name="name">显示名称。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Name(string name)
    {
        if (column != null)
            column.Name = name;
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
    /// 设置表格栏位字段类。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Type(FieldType type)
    {
        if (column != null)
            column.Type = type;
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
    /// 设置表格栏位为排序字段。
    /// </summary>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Sort()
    {
        if (column != null)
            column.IsSort = true;
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