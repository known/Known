using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string id;
    private readonly ColumnInfo column;
    private readonly TableModel<TItem> table;

    internal ColumnBuilder(ColumnInfo column, TableModel<TItem> table = null)
    {
        this.column = column;
        this.table = table;

        if (column != null)
            id = column.Id;
    }

    public ColumnBuilder<TItem> Template(RenderFragment template)
    {
        column.Template = template;
        return this;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        if (string.IsNullOrWhiteSpace(id))
            return this;

        if (table != null)
            table.Templates[id] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }

    public ColumnBuilder<TItem> Width(string width)
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

    public ColumnBuilder<TItem> DefaultAscend() => DefaultSort("asc");
    public ColumnBuilder<TItem> DefaultDescend() => DefaultSort("desc");

    private ColumnBuilder<TItem> DefaultSort(string sort)
    {
        if (column != null)
            column.DefaultSort = sort;
        return this;
    }
}