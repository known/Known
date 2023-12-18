using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class ColumnPanel : BaseComponent
{
    private FieldInfo current;

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public Func<FieldInfo, Task> FieldChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", "字段列表"));
        
        if (Entity == null || Entity.Fields == null || Entity.Fields.Count == 0)
            return;

        builder.Div("columns", () =>
        {
            foreach (var field in Entity.Fields)
            {
                if (field.Id == nameof(EntityBase.Id) ||
                    field.Id == nameof(EntityBase.Version) ||
                    field.Id == nameof(EntityBase.Extension) ||
                    field.Id == nameof(EntityBase.AppId) ||
                    field.Id == nameof(EntityBase.CompNo))
                    continue;

                var active = current?.Id == field.Id ? " active" : "";
                builder.Div($"item{active}", () =>
                {
                    UI.BuildCheckBox(builder, new InputModel<bool>
                    {
                        //Value = 
                    });
                    var text = $"{field.Name}({field.Id})";
                    builder.Span(text, this.Callback(() => OnFieldChanged(field)));
                });
            }
        });
    }

    private Task OnFieldChanged(FieldInfo field)
    {
        current = field;
        return FieldChanged?.Invoke(field);
    }
}