using Known.Blazor;
using Known.Designers;
using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.WorkFlows;

public class BaseFlowForm<TItem> : BaseForm<TItem> where TItem : FlowEntity, new()
{
    private readonly StepModel step = new();
    private readonly TabModel tab = new();

    protected List<ItemModel> Tabs { get; } = [];

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        var logs = await Platform.Flow.GetFlowLogsAsync(Model.Data.Id);
        step.Items.Clear();
        var flow = DataHelper.GetFlow(Model.Table?.Module?.FlowData);
        var steps = flow.GetFlowStepItems();
        if (steps != null && steps.Count > 0)
            step.Items.AddRange(steps);
        step.Current = GetCurrentStep(logs);

        tab.Items.Clear();
        if (Tabs.Count > 0)
            tab.Items.AddRange(Tabs);
        tab.Items.Add(new ItemModel("FlowLog")
        {
            Content = b => b.Component<FlowLogGrid>().Set(c => c.Logs, logs).Build()
        });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildSteps(builder, step);
        UI.BuildTabs(builder, tab);
    }

    private int GetCurrentStep(List<SysFlowLog> logs)
    {
        if (logs != null && logs.Count > 0)
        {
            var last = logs.OrderByDescending(l => l.CreateTime).FirstOrDefault();
            if (last.StepName == FlowStatus.StepEnd && step.Items.Count > 0)
                return step.Items.Count - 1;
        }

        return 0;
    }
}

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    private readonly FlowFormInfo info = new();
    private FlowFormModel flow;

    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitFlowModel();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-content", () => Content?.Invoke(builder));

        if (Model.IsView && Model.FormType == FormType.View)
            return;

        if (Model.FormType != FormType.View)
            BuildFlowAction(builder);

        builder.FormAction(() =>
        {
            if (Model.FormType == FormType.Verify)
                UI.Button(builder, new ActionInfo(Context, "Assign", ""), this.Callback<MouseEventArgs>(OnAssign));

            UI.Button(builder, new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnSaveAsync));
            UI.Button(builder, new ActionInfo(Context, "Cancel", ""), this.Callback<MouseEventArgs>(OnCloseAsync));
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
            case FormType.Submit:
                result = await Platform.Flow.SubmitFlowAsync(info);
                Model.HandleResult(result);
                break;
            case FormType.Verify:
                result = await Platform.Flow.VerifyFlowAsync(info);
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
        if (Model.FormType == FormType.View)
            return;

        info.BizId = Model.Data?.Id;
        flow = new FlowFormModel(Context) { Data = info };

        switch (Model.FormType)
        {
            case FormType.Submit:
                flow.AddUserColumn("SubmitTo", "User");
                flow.AddNoteColumn();
                break;
            case FormType.Verify:
                flow.AddVerifyColumn();
                flow.AddNoteColumn();
                break;
        }
    }
}

class FlowFormModel(Context context) : FormModel<FlowFormInfo>(context, true)
{
    internal void AddUserColumn(string id, string category)
    {
        AddRow().AddColumn(c => c.User, c =>
        {
            c.Id = id;
            c.Required = true;
            c.Template = b =>
            {
                b.Component<Picker<UserPicker, SysUser>>()
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
    private TableModel<SysUser> model;

    public override List<SysUser> SelectedItems => model?.SelectedRows?.ToList();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        model = new TableModel<SysUser>(Context)
        {
            SelectType = "radio",
            ShowPager = true,
            OnQuery = Platform.User.QueryUsersAsync
        };
        model.AddColumn(c => c.UserName).Width("100");
        model.AddColumn(c => c.Name, true).Width("100");
        model.AddColumn(c => c.Phone).Width("100");
        model.AddColumn(c => c.Email).Width("100");
        model.AddColumn(c => c.Role);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.BuildTablePage(model);
}