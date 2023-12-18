using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private string code;

    internal override async Task SetModelAsync(FormInfo model)
    {
        await base.SetModelAsync(model);
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
}