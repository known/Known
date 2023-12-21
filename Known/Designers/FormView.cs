using Known.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, string>> form;
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
        form = new FormModel<Dictionary<string, string>>(UI, Model) { Data = [] };
        foreach (var item in Model.Fields)
        {
            form.Data[item.Id] = $"test-{item.Id}";
        }
    }
}