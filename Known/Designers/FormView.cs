using Known.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, object>> form;
    private string code;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetFormModel();
    }

    internal override void SetModel(FormInfo model)
    {
        base.SetModel(model);
        SetFormModel();
        StateChanged();
    }

    protected override void BuildView(RenderTreeBuilder builder) => UI.BuildForm(builder, form);
    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);

    private void SetFormModel()
    {
        form = new FormModel<Dictionary<string, object>>(UI, Model);
    }
}