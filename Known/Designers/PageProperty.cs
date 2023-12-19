using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageProperty : BaseProperty<PageColumnInfo>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var model = Model ?? new();
        builder.Div("caption", () => builder.Div("title", $"字段属性 - {model.Id}"));
        BuildPropertyItem(builder, "属性", b => b.Span(model.Name));
        BuildPropertyItem(builder, "默认排序", b => UI.BuildSelect(b, new InputModel<string>
        {
            Disabled = IsReadOnly,
            Codes = Cache.GetCodes("升序,降序"),
            Value = model.DefaultSort,
            ValueChanged = this.Callback<string>(value => { Model.DefaultSort = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "查看链接", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.IsViewLink,
            ValueChanged = this.Callback<bool>(value => { Model.IsViewLink = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "查询条件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.IsQuery,
            ValueChanged = this.Callback<bool>(value => { Model.IsQuery = value; OnChanged?.Invoke(Model); })
        }));
        BuildPropertyItem(builder, "显示查询全部", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Disabled = IsReadOnly,
            Value = model.IsQueryAll,
            ValueChanged = this.Callback<bool>(value => { Model.IsQueryAll = value; OnChanged?.Invoke(Model); })
        }));
    }
}