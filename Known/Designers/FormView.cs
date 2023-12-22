using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, object>> form;
    private readonly TableModel<FormFieldInfo> list = new();
    private readonly TabModel tab = new();

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

        tab.Items.Add(new ItemModel("属性") { Content = BuildProperty });
    }

    internal override void SetModel(FormInfo model)
    {
        base.SetModel(model);
        SetModel();
        StateChanged();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () => UI.BuildForm(builder, form));
        builder.Div("setting", () => UI.BuildTabs(builder, tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "标题跨度", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.LabelSpan,
                ValueChanged = this.Callback<int?>(value => { Model.LabelSpan = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "控件跨度", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.WrapperSpan,
                ValueChanged = this.Callback<int?>(value => { Model.WrapperSpan = value; OnPropertyChanged(); })
            }));
        });
    }

    private void SetModel()
    {
        form = new FormModel<Dictionary<string, object>>(UI, Model) { Data = [] };
        form.Initialize();
    }

    private void OnPropertyChanged()
    {
        SetModel(Model);
        OnChanged?.Invoke(Model);
    }
}