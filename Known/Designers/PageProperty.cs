using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageProperty : BaseProperty<PageColumnInfo>
{
    protected override PageColumnInfo GetModel(FieldInfo field)
    {
        return new PageColumnInfo
        {
            Id = field.Id,
            Name = field.Name
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        BuildPropertyItem(builder, "默认排序", b => UI.BuildSelect(b, new InputModel<string>
        {
            Codes = Cache.GetCodes("升序,降序"),
            Value = Model.DefaultSort,
            ValueChanged = this.Callback<string>(value => Model.DefaultSort = value)
        }));
        BuildPropertyItem(builder, "查看链接", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.IsViewLink,
            ValueChanged = this.Callback<bool>(value => Model.IsViewLink = value)
        }));
        BuildPropertyItem(builder, "查询条件", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.IsQuery,
            ValueChanged = this.Callback<bool>(value => Model.IsQuery = value)
        }));
        BuildPropertyItem(builder, "显示查询全部", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Model.IsQueryAll,
            ValueChanged = this.Callback<bool>(value => Model.IsQueryAll = value)
        }));
    }
}