using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormProperty : BaseProperty<FormFieldInfo>
{
    protected override FormFieldInfo GetModel(FieldInfo field)
    {
        return new FormFieldInfo
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type,
            Length = field.Length,
            Required = field.Required
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        BuildPropertyItem(builder, "行序号", b => UI.BuildNumber<int>(b, new InputModel<int>
        {
            Value = Model.Row,
            ValueChanged = this.Callback<int>(value => Model.Row = value)
        }));
        BuildPropertyItem(builder, "列序号", b => UI.BuildNumber<int>(b, new InputModel<int>
        {
            Value = Model.Column,
            ValueChanged = this.Callback<int>(value => Model.Column = value)
        }));
        BuildPropertyItem(builder, "控件类型", b => UI.BuildSelect(b, new InputModel<string>
        {
            Codes = Cache.GetCodes("升序,降序"),
            Value = Model.Type,
            ValueChanged = this.Callback<string>(value => Model.Type = value)
        }));
        BuildPropertyItem(builder, "必填", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.Required,
            ValueChanged = this.Callback<bool>(value => Model.Required = value)
        }));
        BuildPropertyItem(builder, "只读", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.ReadOnly,
            ValueChanged = this.Callback<bool>(value => Model.ReadOnly = value)
        }));
        BuildPropertyItem(builder, "代码类别", b => UI.BuildText(b, new InputModel<string>
        {
            Value = Model.Category,
            ValueChanged = this.Callback<string>(value => Model.Category = value)
        }));
        BuildPropertyItem(builder, "占位符", b => UI.BuildText(b, new InputModel<string>
        {
            Value = Model.Placeholder,
            ValueChanged = this.Callback<string>(value => Model.Placeholder = value)
        }));
        BuildPropertyItem(builder, "多附件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.MultiFile,
            ValueChanged = this.Callback<bool>(value => Model.MultiFile = value)
        }));
    }
}