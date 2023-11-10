using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class ColumnBuilder<TItem>
{
    private string name;
    private TableModel<TItem> table;
    private ColumnAttribute column;

    public ColumnBuilder(TableModel<TItem> table, string name, ColumnAttribute column)
    {
        this.name = name;
        this.table = table;
        this.column = column;
    }

    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        table.Templates[name] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }
}