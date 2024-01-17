using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Designers;

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
        fields = GetFields(Designer.Entity);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", Language["Designer.Fields"]));

        if (fields == null || fields.Count == 0)
            return;

        builder.Div("columns", () =>
        {
            foreach (var field in fields)
            {
                var active = current?.Id == field.Id ? " active" : "";
                builder.Div($"item{active}", () =>
                {
                    UI.BuildCheckBox(builder, new InputModel<bool>
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
            Designer.Fields.Remove(field);

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

    private List<FieldInfo> GetFields(EntityInfo info)
    {
        var infos = new List<FieldInfo>();
        if (info == null)
            return infos;

        foreach (var field in info.Fields)
        {
            infos.Add(new FieldInfo { Id = field.Id, Name = field.Name, Type = field.Type, Required = field.Required });
        }

        if (info.IsFlow)
        {
            infos.Add(GetFieldInfo(nameof(FlowEntity.BizStatus)));
            infos.Add(GetFieldInfo(nameof(FlowEntity.ApplyBy)));
            infos.Add(GetFieldInfo(nameof(FlowEntity.ApplyTime)));
            infos.Add(GetFieldInfo(nameof(FlowEntity.VerifyBy)));
            infos.Add(GetFieldInfo(nameof(FlowEntity.VerifyTime)));
            infos.Add(GetFieldInfo(nameof(FlowEntity.VerifyNote)));
        }

        infos.Add(GetFieldInfo(nameof(EntityBase.CreateBy)));
        infos.Add(GetFieldInfo(nameof(EntityBase.CreateTime)));
        infos.Add(GetFieldInfo(nameof(EntityBase.ModifyBy)));
        infos.Add(GetFieldInfo(nameof(EntityBase.ModifyTime)));

        return infos;
    }

    private FieldInfo GetFieldInfo(string id) => new FieldInfo { Id = id, Name = Language[id] };
}