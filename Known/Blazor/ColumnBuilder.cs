namespace Known.Blazor;

public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string id;
    private readonly ColumnInfo column;

    public TableModel<TItem> Table { get; }

    internal ColumnBuilder(ColumnInfo column, TableModel<TItem> table = null)
    {
        this.column = column;
        Table = table;

        if (column != null)
            id = column.Id;
    }

    public ColumnBuilder<TItem> Template(RenderFragment template)
    {
        if (column != null)
            column.Template = template;
        return this;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        if (string.IsNullOrWhiteSpace(id))
            return this;

        if (Table != null)
            Table.Templates[id] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }

    public ColumnBuilder<TItem> Width(int width)
    {
        if (column != null)
            column.Width = width;
        return this;
    }

    public ColumnBuilder<TItem> Visible(bool visible)
    {
        if (column != null)
            column.IsVisible = visible;
        return this;
    }

    public ColumnBuilder<TItem> ReadOnly(bool readOnly)
    {
        if (column != null)
            column.ReadOnly = readOnly;
        return this;
    }

    public ColumnBuilder<TItem> ViewLink()
    {
        if (column != null)
            column.IsViewLink = true;
        return this;
    }

    public ColumnBuilder<TItem> Sum()
    {
        if (column != null)
            column.IsSum = true;
        return this;
    }

    public ColumnBuilder<TItem> Name(string name)
    {
        if (column != null)
            column.Name = name;
        return this;
    }
    
    public ColumnBuilder<TItem> Tooltip(string tooltip)
    {
        if (column != null)
            column.Tooltip = tooltip;
        return this;
    }

    public ColumnBuilder<TItem> Category(string category, bool isAll = true)
    {
        if (column != null)
        {
            column.Category = category;
            column.IsQueryAll = isAll;
        }
        return this;
    }

    public ColumnBuilder<TItem> Align(string align)
    {
        if (column != null)
            column.Align = align;
        return this;
    }

    public ColumnBuilder<TItem> Fixed(string fixType)
    {
        if (column != null)
            column.Fixed = fixType;
        return this;
    }

    public ColumnBuilder<TItem> Type(FieldType type)
    {
        if (column != null)
            column.Type = type;
        return this;
    }

    public ColumnBuilder<TItem> Sort()
    {
        if (column != null)
            column.IsSort = true;
        return this;
    }

    public ColumnBuilder<TItem> DefaultAscend() => DefaultSort("asc");
    public ColumnBuilder<TItem> DefaultDescend() => DefaultSort("desc");

    private ColumnBuilder<TItem> DefaultSort(string sort)
    {
        if (column != null)
            column.DefaultSort = sort;
        return this;
    }
}