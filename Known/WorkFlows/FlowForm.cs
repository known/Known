namespace Known.WorkFlows;

public class BaseFlowForm<TItem> : BaseTabForm where TItem : FlowEntity, new()
{
    private IFlowService Flow;
    private readonly StepModel step = new();

    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Flow = await CreateServiceAsync<IFlowService>();
        Tab.AddTab("FlowLog", b => b.Component<FlowLogGrid>().Set(c => c.BizId, Model.Data.Id).Build());

        step.Items.Clear();
        var flow = await Flow.GetFlowAsync(Context.Current.Id, Model.Data.Id);
        var steps = flow.GetFlowStepItems();
        if (steps != null && steps.Count > 0)
            step.Items.AddRange(steps);
        step.Current = flow.Current;
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        UI.BuildSteps(builder, step);
        UI.BuildTabs(builder, Tab);
    }
}

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    private IFlowService Service;
    private readonly FlowFormInfo info = new();
    private FlowFormModel flow;

    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        InitFlowModel();
        Service = await CreateServiceAsync<IFlowService>();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-content", () => Content?.Invoke(builder));

        if (Model.IsView && Model.FormType == FormViewType.View)
            return;

        if (Model.FormType != FormViewType.View)
            BuildFlowAction(builder);

        builder.FormAction(() =>
        {
            if (Model.FormType == FormViewType.Verify)
                builder.Button(new ActionInfo(Context, "Assign", ""), this.Callback<MouseEventArgs>(OnAssign));

            builder.Button(new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnSaveAsync));
            builder.Button(new ActionInfo(Context, "Cancel", ""), this.Callback<MouseEventArgs>(OnCloseAsync));
        });
    }

    private void BuildFlowAction(RenderTreeBuilder builder)
    {
        var action = Language[$"Button.{Model.FormType}"];
        var title = Language["Title.FlowAction"].Replace("{action}", action);
        builder.Div("kui-flow", () => builder.GroupBox(title, () => UI.BuildForm(builder, flow)));
    }

    private void OnAssign(MouseEventArgs args) => Model.Page.AssignFlow(Model.Data);

    private async void OnSaveAsync(MouseEventArgs args)
    {
        if (flow != null && !flow.Validate())
            return;

        Result result;
        switch (Model.FormType)
        {
            case FormViewType.Submit:
                result = await Service.SubmitFlowAsync(info);
                Model.HandleResult(result);
                break;
            case FormViewType.Verify:
                result = await Service.VerifyFlowAsync(info);
                Model.HandleResult(result);
                break;
            default:
                await Model.SaveAsync();
                break;
        }
    }

    private async void OnCloseAsync(MouseEventArgs args) => await Model.CloseAsync();

    private void InitFlowModel()
    {
        if (Model.FormType == FormViewType.View)
            return;

        info.BizId = Model.Data?.Id;
        flow = new FlowFormModel(Context) { Data = info };

        switch (Model.FormType)
        {
            case FormViewType.Submit:
                flow.AddUserColumn("SubmitTo", "User");
                flow.AddNoteColumn();
                break;
            case FormViewType.Verify:
                flow.AddVerifyColumn();
                flow.AddNoteColumn();
                break;
        }
    }
}

class FlowFormModel(UIContext context) : FormModel<FlowFormInfo>(context, true)
{
    internal void AddUserColumn(string id, string category)
    {
        AddRow().AddColumn(c => c.User, c =>
        {
            c.Id = id;
            c.Required = true;
            c.Template = b =>
            {
                b.Component<KPicker<UserPicker, SysUser>>()
                 .Set(c => c.Width, 800)
                 .Set(c => c.AllowClear, true)
                 .Set(c => c.Title, Language["Title.SelectUser"])
                 .Set(c => c.Value, Data.User)
                 .Set(c => c.OnPicked, o => Data.User = o?[0]?.UserName)
                 .Build();
            };
        });
    }

    internal void AddVerifyColumn()
    {
        AddRow().AddColumn(c => c.BizStatus, c =>
        {
            c.Id = "VerifyResult";
            c.Required = true;
            c.Type = FieldType.RadioList;
            c.Category = $"{FlowStatus.VerifyPass},{FlowStatus.VerifyFail}";
        });
    }

    internal void AddNoteColumn()
    {
        AddRow().AddColumn(c => c.Note, c =>
        {
            c.Id = "Note";
            c.Type = FieldType.TextArea;
        });
    }

    internal void AddReasonColumn(string name)
    {
        var reason = Language?["XXReason"]?.Replace("{name}", name);
        AddRow().AddColumn(c => c.Note, c =>
        {
            c.Name = reason;
            c.Required = true;
            c.Type = FieldType.TextArea;
        });
    }
}

class UserPicker : BasePicker<SysUser>
{
    private IUserService Service;

    protected override async Task OnInitAsync()
    {
        IsMulti = false;
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IUserService>();
        Table.OnQuery = Service.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }
}