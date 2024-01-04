using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FormView : BaseView<FormInfo>
{
    private FormModel<Dictionary<string, object>> form;
    private readonly TableModel<FormFieldInfo> list = new();
    private readonly TabModel tab = new();

    [Parameter] public FlowInfo Flow { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetModel();
        Tab.Items.Add(new ItemModel(Language["Designer.View"]) { Content = BuildView });
        Tab.Items.Add(new ItemModel(Language["Designer.Fields"]) { Content = BuildList });

        list.FixedHeight = "380px";
        list.OnQuery = c =>
        {
            var result = new PagingResult<FormFieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };

        tab.Items.Add(new ItemModel(Language["Designer.Property"]) { Content = BuildProperty });
    }

    internal override void SetModel(FormInfo model)
    {
        base.SetModel(model);
        SetModel();
        StateChanged();
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
            BuildPropertyItem(builder, Language["Designer.LabelSpan"], b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.LabelSpan,
                ValueChanged = this.Callback<int?>(value => { Model.LabelSpan = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, Language["Designer.WrapperSpan"], b => UI.BuildNumber(b, new InputModel<int?>
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