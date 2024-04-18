namespace Known.Designers;

class FlowDesigner : BaseDesigner<string>
{
    private List<CodeInfo> addTypes;
    private List<CodeInfo> flowModels;
    private string addType;
    private FlowInfo flow = new();
    private FlowView view;

    private bool IsNew => addType == addTypes[0].Code;
    private bool IsReadOnly => ReadOnly || !Entity.IsFlow;

    protected override async Task OnInitAsync()
    {
        addTypes =
        [
            new CodeInfo("New", Language["Designer.New"]),
            new CodeInfo("Select", Language["Designer.SelectFlow"])
        ];
        await base.OnInitAsync();
        flowModels = DataHelper.Flows.Select(m => new CodeInfo(m.Id, $"{m.Name}({m.Id})", m)).ToList();
        addType = string.IsNullOrWhiteSpace(Model) || Model.Contains('|')
                ? addTypes[0].Code : addTypes[1].Code;
        flow = DataHelper.GetFlow(Model);
        Form.Flow = flow;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-designer entity", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Div("caption", () =>
                {
                    builder.Div("title", Language["Designer.FlowModel"]);
                    BuildModelType(builder);
                });
                BuildNewModel(builder);
            });
            builder.Div("panel-view", () =>
            {
                builder.Component<FlowView>().Set(c => c.Model, flow).Build(value => view = value);
            });
        });
    }

    private void BuildModelType(RenderTreeBuilder builder)
    {
        builder.Div(() =>
        {
            UI.BuildRadioList(builder, new InputModel<string>
            {
                Disabled = IsReadOnly,
                Codes = addTypes,
                Value = addType,
                ValueChanged = this.Callback<string>(OnTypeChanged)
            });
        });

        if (!IsNew)
        {
            builder.Div("select", () =>
            {
                UI.BuildSelect(builder, new InputModel<string>
                {
                    Disabled = IsReadOnly,
                    Codes = flowModels,
                    Value = Model,
                    ValueChanged = this.Callback<string>(OnModelChanged)
                });
            });
        }
    }

    private void BuildNewModel(RenderTreeBuilder builder)
    {
        builder.Markup($@"<pre><b>{Language["Designer.Explanation"]}</b>
{Language["Designer.Flow"]}{Language["Name"]}|{Language["Code"]}
{Language["Designer.Step"]}{Language["Name"]}|{Language["Code"]}|{Language["User"]}|{Language["Role"]}|{Language["Pass"]}|{Language["Fail"]}
<b>{Language["Designer.Sample"]}</b>
{Language["BizApply"]}|BizApplyFlow
{Language["BizApply"]}|Apply|||{Language["Flow.Verifing"]}
{Language["BizVerify"]}|Verify||VerifyBy|{Language["Flow.Pass"]}|{Language["Flow.Fail"]}
{Language["Flow.End"]}|End</pre>");
        UI.BuildTextArea(builder, new InputModel<string>
        {
            Disabled = ReadOnly || !IsNew,
            Rows = 11,
            Value = Model,
            ValueChanged = this.Callback<string>(OnModelChanged)
        });
    }

    private void OnTypeChanged(string type) => addType = type;

    private void OnModelChanged(string model)
    {
        Model = model;
        flow = DataHelper.GetFlow(model);
        Form.Flow = flow;
        view?.SetModel(flow);
        OnChanged?.Invoke(model);
    }
}