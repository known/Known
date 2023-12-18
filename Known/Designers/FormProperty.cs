using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormProperty : BaseProperty
{
    [Parameter] public ColumnInfo Column { get; set; } = new();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        BuildPropertyItem(builder, "行序号", b => UI.BuildNumber<int>(b, new InputModel<int>
        {
            Value = Column.Row,
            ValueChanged = this.Callback<int>(value => Column.Row = value)
        }));
        BuildPropertyItem(builder, "列序号", b => UI.BuildNumber<int>(b, new InputModel<int>
        {
            Value = Column.Column,
            ValueChanged = this.Callback<int>(value => Column.Column = value)
        }));
        BuildPropertyItem(builder, "控件类型", b => UI.BuildSelect(b, new InputModel<string>
        {
            Codes = Cache.GetCodes("升序,降序"),
            Value = Column.DefaultSort,
            ValueChanged = this.Callback<string>(value => Column.DefaultSort = value)
        }));
        BuildPropertyItem(builder, "必填", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsRequired,
            ValueChanged = this.Callback<bool>(value => Column.IsRequired = value)
        }));
        BuildPropertyItem(builder, "只读", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsReadOnly,
            ValueChanged = this.Callback<bool>(value => Column.IsReadOnly = value)
        }));
        BuildPropertyItem(builder, "代码类别", b => UI.BuildText(b, new InputModel<string>
        {
            Value = Column.Category,
            ValueChanged = this.Callback<string>(value => Column.Category = value)
        }));
        BuildPropertyItem(builder, "占位符", b => UI.BuildText(b, new InputModel<string>
        {
            Value = Column.Placeholder,
            ValueChanged = this.Callback<string>(value => Column.Placeholder = value)
        }));
        BuildPropertyItem(builder, "单附件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsFile,
            ValueChanged = this.Callback<bool>(value => Column.IsFile = value)
        }));
        BuildPropertyItem(builder, "多附件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsMultiFile,
            ValueChanged = this.Callback<bool>(value => Column.IsMultiFile = value)
        }));
    }
}