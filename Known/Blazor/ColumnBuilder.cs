using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string name;
    private readonly TableModel<TItem> table;
    private readonly ColumnInfo column;

    internal ColumnBuilder(TableModel<TItem> table, ColumnInfo column)
    {
        this.table = table;
        this.column = column;

        if (column != null)
            name = column.Property.Name;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        if (string.IsNullOrWhiteSpace(name))
            return this;

        table.Templates[name] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }

    public ColumnBuilder<TItem> Visible(bool visible)
    {
        if (column != null)
            column.IsGrid = visible;
        return this;
    }

    public ColumnBuilder<TItem> ReadOnly(bool readOnly)
    {
        if (column != null)
            column.IsReadOnly = readOnly;
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