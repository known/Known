using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormDesigner : BaseDesigner
{
    private BaseView view;
    private BaseProperty property;

    [Parameter] public FormInfo Model { get; set; }
    [Parameter] public Action<FormInfo> OnChanged { get; set; }

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

    protected override void FieldChanged(FieldInfo field)
    {
        view?.StateChanged();
        property?.StateChanged();
    }
}