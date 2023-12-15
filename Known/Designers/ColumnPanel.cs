using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class ColumnPanel : ComponentBase
{
    private ColumnInfo current;

    [Parameter] public List<ColumnInfo> Columns { get; set; }
    [Parameter] public Func<ColumnInfo, Task> ColumnChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Columns == null || Columns.Count == 0)
            return;

        builder.Div("columns", () =>
        {
            foreach (var column in Columns)
            {
                if (column.Id == nameof(EntityBase.Id) ||
                    column.Id == nameof(EntityBase.Version) ||
                    column.Id == nameof(EntityBase.Extension) ||
                    column.Id == nameof(EntityBase.AppId) ||
                    column.Id == nameof(EntityBase.CompNo))
                    continue;

                var className = current?.Id == column.Id ? "active" : "";
                var text = $"{column.Name}({column.Id})";
                builder.Span(className, text, this.Callback(() => OnColumnChanged(column)));
            }
        });
    }

    private Task OnColumnChanged(ColumnInfo column)
    {
        current = column;
        return ColumnChanged?.Invoke(column);
    }
}