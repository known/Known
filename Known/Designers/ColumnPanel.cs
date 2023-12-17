using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class ColumnPanel : BaseComponent
{
    private ColumnInfo current;

    [Parameter] public List<ColumnInfo> Columns { get; set; }
    [Parameter] public Func<ColumnInfo, Task> ColumnChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", "字段列表"));
        
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

                var active = current?.Id == column.Id ? " active" : "";
                builder.Div($"item{active}", () =>
                {
                    UI.BuildCheckBox(builder, new InputModel<bool>
                    {
                        //Value = 
                    });
                    var text = $"{column.Name}({column.Id})";
                    builder.Span(text, this.Callback(() => OnColumnChanged(column)));
                });
            }
        });
    }

    private Task OnColumnChanged(ColumnInfo column)
    {
        current = column;
        return ColumnChanged?.Invoke(column);
    }
}