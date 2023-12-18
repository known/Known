using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class ColumnPanel : BaseComponent
{
    private FieldInfo current;

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public List<FieldInfo> Fields { get; set; } = [];
    [Parameter] public Action OnFieldCheck { get; set; }
    [Parameter] public Action<FieldInfo> OnFieldClick { get; set; }

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
                        Disabled = ReadOnly,
                        Value = Fields.Contains(field),
                        ValueChanged = this.Callback<bool>(c => OnFieldChecked(field, c))
                    });
                    var text = $"{field.Name}({field.Id})";
                    builder.Span(text, this.Callback(() => OnFieldClicked(field)));
                });
            }
        });
    }

    private void OnFieldChecked(FieldInfo field, bool isCheck)
    {
        if (isCheck)
        {
            if (!Fields.Contains(field))
                Fields.Add(field);
        }
        else
        {
            Fields.Remove(field);
        }

        OnFieldCheck?.Invoke();
    }

    private void OnFieldClicked(FieldInfo field)
    {
        current = field;
        OnFieldClick?.Invoke(field);
    }
}