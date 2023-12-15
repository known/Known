using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageProperty : BaseProperty
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        BuildPropertyItem(builder, "默认排序", b => UI.BuildSelect(b, new InputModel<string>
        {
            Codes = Cache.GetCodes("升序,降序"),
            Value = Column.DefaultSort,
            ValueChanged = this.Callback<string>(value => Column.DefaultSort = value)
        }));
        BuildPropertyItem(builder, "查看链接", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsViewLink,
            ValueChanged = this.Callback<bool>(value => Column.IsViewLink = value)
        }));
        BuildPropertyItem(builder, "查询", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsQuery,
            ValueChanged = this.Callback<bool>(value => Column.IsQuery = value)
        }));
        BuildPropertyItem(builder, "全部查询", b => UI.BuildSwitch(b, new InputModel<bool>
        {
            Value = Column.IsQueryAll,
            ValueChanged = this.Callback<bool>(value => Column.IsQueryAll = value)
        }));
    }
}