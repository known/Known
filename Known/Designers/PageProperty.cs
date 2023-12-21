using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageProperty : BaseProperty<PageColumnInfo>
{
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {Model.Id}"));
        BuildPropertyItem(builder, "显示名称", b => b.Span(Model.Name));
        BuildPropertyItem(builder, "查看链接", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = Model.IsViewLink,
            ValueChanged = this.Callback<bool>(value => { Model.IsViewLink = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "查询条件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = Model.IsQuery,
            ValueChanged = this.Callback<bool>(value => { Model.IsQuery = value; OnChanged?.Invoke(Model); })
        }));
        if (Model.IsQuery)
        {
            BuildPropertyItem(builder, "显示全部", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = IsReadOnly,
                Value = Model.IsQueryAll,
                ValueChanged = this.Callback<bool>(value => { Model.IsQueryAll = value; OnChanged?.Invoke(Model); })
            }));
        }
        BuildPropertyItem(builder, "排序", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = Model.IsSort,
            ValueChanged = this.Callback<bool>(value => { Model.IsSort = value; OnChanged?.Invoke(Model); })
        }));
        if (Model.IsSort)
        {
            BuildPropertyItem(builder, "默认排序", b => UI.BuildSelect(b, new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = Cache.GetCodes(",升序,降序"),
                Value = Model.DefaultSort,
                ValueChanged = this.Callback<string>(value => { Model.DefaultSort = value; OnChanged?.Invoke(Model); })
            }));
        }
    }
}