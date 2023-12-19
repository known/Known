using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormProperty : BaseProperty<FormFieldInfo>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var model = Model ?? new();
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {model.Id}"));
        BuildPropertyItem(builder, "属性", b => b.Span(model.Name));
        BuildPropertyItem(builder, "行序号", b => UI.BuildNumber(b, new InputModel<int>
        {
            Disabled = IsReadOnly,
            Value = model.Row,
            ValueChanged = this.Callback<int>(value => { Model.Row = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "列序号", b => UI.BuildNumber(b, new InputModel<int>
        {
            Disabled = IsReadOnly,
            Value = model.Column,
            ValueChanged = this.Callback<int>(value => { Model.Column = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "控件类型", b => UI.BuildSelect(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Codes = Cache.GetCodes(EntityDesigner.DataTypes),
            Value = model.Type,
            ValueChanged = this.Callback<string>(value => { Model.Type = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "必填", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.Required,
            ValueChanged = this.Callback<bool>(value => { Model.Required = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "只读", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.ReadOnly,
            ValueChanged = this.Callback<bool>(value => { Model.ReadOnly = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "代码类别", b => UI.BuildText(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Value = model.Category,
            ValueChanged = this.Callback<string>(value => { Model.Category = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "占位符", b => UI.BuildText(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Value = model.Placeholder,
            ValueChanged = this.Callback<string>(value => { Model.Placeholder = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "多附件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.MultiFile,
            ValueChanged = this.Callback<bool>(value => { Model.MultiFile = value; OnChanged?.Invoke(Model); })
        }));
    }
}