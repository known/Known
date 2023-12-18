using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private string code;

    internal override async Task SetModelAsync(PageInfo model)
    {
        await base.SetModelAsync(model);
        code = Service.GetPage(Model);
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
}