using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormDesigner : BaseDesigner<FormInfo>
{
    protected override void BuildDesigner(RenderTreeBuilder builder)
    {
        builder.Div("panel-view", () =>
        {
            builder.Component<FormView>().Build(value => view = value);
        });
        builder.Div("panel-property", () =>
        {
            builder.Component<FormProperty>().Build(value => property = value);
        });
    }
}