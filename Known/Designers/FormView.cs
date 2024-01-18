using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, object>> form;
    private TableModel<FormFieldInfo> list;
    private readonly TabModel tab = new();

    [Parameter] public FlowInfo Flow { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        form = new FormModel<Dictionary<string, object>>(Context, false) { Data = [] };
        SetForm();

        Tab.Items.Add(new ItemModel("Designer.View") { Content = BuildView });
        Tab.Items.Add(new ItemModel("Designer.Fields") { Content = BuildList });

        list = new(Context, true);
        list.FixedHeight = "380px";
        list.OnQuery = c =>
        {
            var result = new PagingResult<FormFieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };

        tab.Items.Add(new ItemModel("Designer.Property") { Content = BuildProperty });
    }

    internal override async void SetModel(FormInfo model)
    {
        base.SetModel(model);
        SetForm();
        StateChanged();
        await list.RefreshAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () =>
        {
            var steps = Flow?.GetFlowStepItems();
            if (steps != null && steps.Count > 0)
            {
                var step = new StepModel();
                step.Items.AddRange(steps);
                UI.BuildSteps(builder, step);
            }
            UI.BuildForm(builder, form);
        });
        builder.Div("setting", () => UI.BuildTabs(builder, tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "Designer.LabelSpan", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.LabelSpan,
                ValueChanged = this.Callback<int?>(value => { Model.LabelSpan = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.WrapperSpan", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.WrapperSpan,
                ValueChanged = this.Callback<int?>(value => { Model.WrapperSpan = value; OnPropertyChanged(); })
            }));
        });
    }

    private void OnPropertyChanged()
    {
        SetModel(Model);
        OnChanged?.Invoke(Model);
    }

    private void SetForm()
    {
        form.SetFormInfo(Model);
        form.InitColumns();
    }
}