using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string name;
    private readonly TableModel<TItem> table;
    private readonly ColumnAttribute column;

    internal ColumnBuilder(TableModel<TItem> table, ColumnAttribute column)
    {
        name = column.Property.Name;
        this.table = table;
        this.column = column;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        table.Templates[name] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }

    public ColumnBuilder<TItem> Visible(bool visible)
    {
        column.IsGrid = visible;
        return this;
    }

    public ColumnBuilder<TItem> DefaultSort(string sort)
    {
        column.DefaultSort = sort;
        return this;
    }
}