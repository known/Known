using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class ColumnBuilder<TItem> where TItem : class, new()
{
    private readonly string name;
    private readonly TableModel<TItem> table;
    private readonly ColumnAttribute column;

    internal ColumnBuilder(TableModel<TItem> table, ColumnAttribute column)
    {
        this.name = column.Property.Name;
        this.table = table;
        this.column = column;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        table.Templates[name] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }
}