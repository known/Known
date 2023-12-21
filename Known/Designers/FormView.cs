using Known.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, object>> form;
    private readonly TableModel<FormFieldInfo> list = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetModel();
        Tab.Items.Add(new ItemModel("视图") { Content = BuildView });
        Tab.Items.Add(new ItemModel("字段") { Content = BuildList });

        list.ScrollY = "380px";
        list.OnQuery = c =>
        {
            var result = new PagingResult<FormFieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };
    }

    internal override void SetModel(FormInfo model)
    {
        base.SetModel(model);
        SetModel();
        StateChanged();
    }

    private void BuildView(RenderTreeBuilder builder) => UI.BuildForm(builder, form);
    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void SetModel()
    {
        form = new FormModel<Dictionary<string, object>>(UI, Model) { Data = [] };
        form.Initialize();
    }
}