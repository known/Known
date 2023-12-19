using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private string code;

    internal override void SetModel(FormInfo model)
    {
        base.SetModel(model);
        StateChanged();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        foreach (var item in Model.Fields)
        {
            builder.Div("", Utils.ToJson(item));
        }
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
}