using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class ColumnPanel : BaseComponent
{
    private FieldInfo current;
    private List<FieldInfo> fields;

    [Parameter] public EntityInfo Entity { get; set; }
    [Parameter] public List<FieldInfo> Fields { get; set; } = [];
    [Parameter] public Action<FieldInfo> OnFieldCheck { get; set; }
    [Parameter] public Action<FieldInfo> OnFieldClick { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        current = Fields?.FirstOrDefault();
        fields = GetFields(Entity);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", "字段列表"));

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
                        Value = Fields.Exists(f => f.Id == field.Id),
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
        var info = Fields.FirstOrDefault(f => f.Id == field.Id);
        if (isCheck)
        {
            if (info == null) Fields.Add(field);
        }
        else
        {
            Fields.Remove(info);
        }

        current = field;
        OnFieldCheck?.Invoke(field);
        OnFieldClick?.Invoke(field);
    }

    private void OnFieldClicked(FieldInfo field)
    {
        current = field;
        OnFieldClick?.Invoke(field);
    }

    private static List<FieldInfo> GetFields(EntityInfo info)
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
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.BizStatus), Name = "流程状态" });
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.ApplyBy), Name = "申请人" });
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.ApplyTime), Name = "申请时间" });
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.VerifyBy), Name = "审核人" });
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.VerifyTime), Name = "审核时间" });
            infos.Add(new FieldInfo { Id = nameof(FlowEntity.VerifyNote), Name = "审核意见" });
        }

        infos.Add(new FieldInfo { Id = nameof(EntityBase.CreateBy), Name = "创建人" });
        infos.Add(new FieldInfo { Id = nameof(EntityBase.CreateTime), Name = "创建时间" });
        infos.Add(new FieldInfo { Id = nameof(EntityBase.ModifyBy), Name = "修改人" });
        infos.Add(new FieldInfo { Id = nameof(EntityBase.ModifyTime), Name = "修改时间" });

        return infos;
    }
}