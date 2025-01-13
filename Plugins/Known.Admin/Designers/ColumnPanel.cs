﻿namespace Known.Designers;

class ColumnPanel<TModel> : BaseComponent
{
    private FieldInfo current;
    private List<FieldInfo> fields;

    [CascadingParameter] private BaseViewDesigner<TModel> Designer { get; set; }

    [Parameter] public Action<FieldInfo> OnFieldCheck { get; set; }
    [Parameter] public Action<FieldInfo> OnFieldClick { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        current = Designer.Fields?.FirstOrDefault();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        //TODO：Page支持添加栏位以及位置移动
        builder.Div("caption", () => builder.Div("title", Language["Designer.Fields"]));

        fields = Designer.Module.Entity.GetFields(Language);
        if (fields == null || fields.Count == 0)
            return;

        builder.Div("columns", () =>
        {
            foreach (var field in fields)
            {
                var active = current?.Id == field.Id ? " active" : "";
                builder.Div($"item{active}", () =>
                {
                    builder.CheckBox(new InputModel<bool>
                    {
                        Disabled = ReadOnly,
                        Value = Designer.Fields.Exists(f => f.Id == field.Id),
                        ValueChanged = this.Callback<bool>(c => OnFieldChecked(field, c))
                    });
                    var text = $"{field.Name}({field.Id})";
                    builder.Span(text, this.Callback<MouseEventArgs>(e => OnFieldClicked(field)));
                });
            }
        });
    }

    private void OnFieldChecked(FieldInfo field, bool isCheck)
    {
        if (!isCheck)
        {
            var info = Designer.Fields.FirstOrDefault(f => f.Id == field.Id);
            if (info != null)
                Designer.Fields.Remove(info);
        }

        var infos = new List<FieldInfo>();
        foreach (var item in fields)
        {
            if (isCheck && item.Id == field.Id || Designer.Fields.Exists(f => f.Id == item.Id))
                infos.Add(item);
        }
        Designer.Fields = infos;

        current = field;
        OnFieldCheck?.Invoke(field);
        OnFieldClick?.Invoke(field);
    }

    private void OnFieldClicked(FieldInfo field)
    {
        current = field;
        OnFieldClick?.Invoke(field);
    }
}