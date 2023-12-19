using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private string code;

    internal override void SetModel(PageInfo model)
    {
        base.SetModel(model);
        code = Service.GetPage(Model);
        StateChanged();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        foreach (var item in Model.Columns)
        {
            builder.Div("", Utils.ToJson(item));
        }
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
}