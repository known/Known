using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormProperty : BaseProperty<FormFieldInfo>
{
    private List<CodeInfo> controlTypes;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        controlTypes = Cache.GetCodes(nameof(FieldType)).Select(c => new CodeInfo(c.Name, c.Name)).ToList();
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {Model.Id}"));
        BuildPropertyItem(builder, "显示名称", b => b.Span(Model.Name));
        BuildPropertyItem(builder, "行序号", b => UI.BuildNumber(b, new InputModel<int>
        {
            Disabled = IsReadOnly,
            Value = Model.Row,
            ValueChanged = this.Callback<int>(value => { Model.Row = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "列序号", b => UI.BuildNumber(b, new InputModel<int>
        {
            Disabled = IsReadOnly,
            Value = Model.Column,
            ValueChanged = this.Callback<int>(value => { Model.Column = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "控件类型", b => UI.BuildSelect(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Codes = controlTypes,
            Value = Model.Type.ToString(),
            ValueChanged = this.Callback<string>(value =>
            {
                if (Model != null)
                    Model.Type = Utils.ConvertTo<FieldType>(value);
                OnChanged?.Invoke(Model);
            })
        }));
        BuildPropertyItem(builder, "必填", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = Model.Required,
            ValueChanged = this.Callback<bool>(value => { Model.Required = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "只读", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = Model.ReadOnly,
            ValueChanged = this.Callback<bool>(value => { Model.ReadOnly = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "占位符", b => UI.BuildText(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Value = Model.Placeholder,
            ValueChanged = this.Callback<string>(value => { Model.Placeholder = value; OnChanged?.Invoke(Model); })
        }));
        if (Model.Type == FieldType.Select || Model.Type == FieldType.RadioList || Model.Type == FieldType.CheckList)
        {
            BuildPropertyItem(builder, "代码类别", b => UI.BuildText(b, new InputModel<string>
            {
                Disabled = IsReadOnly,
                Value = Model.Category,
                ValueChanged = this.Callback<string>(value => { Model.Category = value; OnChanged?.Invoke(Model); })
            }));
        }
        if (Model.Type == FieldType.File)
        {
            BuildPropertyItem(builder, "多附件", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.MultiFile,
                ValueChanged = this.Callback<bool>(value => { Model.MultiFile = value; OnChanged?.Invoke(Model); })
            }));
        }
    }
}