using Known.Razor.Pages;

namespace Sample.Razor.BizApply.Forms;

[Dialog(960, 600)]
class ApplyForm : WebForm<TbApply>
{
    private TbApply? model;

    public ApplyForm()
    {
        TabItems = new List<MenuItem>
        {
            new MenuItem("BaseInfo", "基本信息"),
            new MenuItem("FlowLog", "流程记录")
        };
    }

    [Parameter] public PageType PageType { get; set; }
    [Parameter] public SysFlow Flow { get; set; }

    protected override async Task InitFormAsync()
    {
        if (Flow != null)
            Model = await Client.Apply.GetApplyAsync(Flow.BizId);

        model = TModel;
        await base.InitFormAsync();
    }

    protected override void BuildFields(FieldBuilder<TbApply> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(15, 35, 15, 35);
            table.Tr(attr =>
            {
                table.Field<Text>(f => f.BizNo).Enabled(false).Build();
                table.Field<Text>(f => f.BizTitle).Build();
            });
            table.Tr(attr =>
            {
                table.Field<RichText>(f => f.BizContent).ColSpan(3)
                     .Set(f => f.Option, new
                     {
                         Height = 150,
                         Placeholder = "请输入申请单内容"
                     })
                     .Build();
            });
            table.Tr(attr =>
            {
                table.Field<Upload>(f => f.BizFile).ColSpan(3)
                     .Set(f => f.IsMultiple, true)
                     .Set(f => f.CanDelete, true)
                     .Build();
            });
        });
    }

    protected override void BuildTabBody(RenderTreeBuilder builder, MenuItem item)
    {
        if (item.Name == "流程记录")
        {
            builder.Component<FlowLogGrid>()
                   .Set(c => c.BizId, model?.Id)
                   .Build();
        }
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        if (PageType == PageType.Apply)
        {
            builder.Button(ToolButton.Submit, Callback(OnSubmitFlow));
        }
        else if (PageType == PageType.Verify)
        {
            //审核页面显示通过和退回按钮
            builder.Button(ToolButton.Assign, Callback(OnAssignFlow));
            builder.Button(ToolButton.Pass, Callback(OnPassFlow));
            builder.Button(ToolButton.Return, Callback(OnReturnFlow));
        }

        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSubmitFlow()
    {
        if (model.IsNew)
        {
            UI.Toast("请先保存记录再提交审核！");
            return;
        }

        if (!model.CanSubmit)
        {
            UI.Toast($"{model.BizStatus}记录不能提交审核！");
            return;
        }

        var vr = model.ValidCommit();
        if (!vr.IsValid)
        {
            UI.Alert(vr.Message);
            return;
        }

        UI.SubmitFlow(Platform.Flow, new FlowFormInfo
        {
            UserRole = UserRole.Verifier,
            BizId = model.Id,
            BizStatus = FlowStatus.Verifing,
            Model = Model
        }, OnSuccessed);
    }

    private void OnAssignFlow()
    {
        UI.AssignFlow(Platform.Flow, new FlowFormInfo
        {
            UserRole = UserRole.Verifier,
            BizId = model?.Id,
            BizStatus = model?.BizStatus,
            Model = Model
        }, OnSuccessed);
    }

    private void OnPassFlow()
    {
        if (!ValidateCheck(true))
            return;

        UI.VerifyFlow(Platform.Flow, new FlowFormInfo
        {
            BizId = model?.Id,
            BizStatus = FlowStatus.VerifyPass,
            Model = Model
        }, OnSuccessed);
    }

    private void OnReturnFlow()
    {
        if (!ValidateCheck(false))
            return;

        UI.VerifyFlow(Platform.Flow, new FlowFormInfo
        {
            BizId = model?.Id,
            BizStatus = FlowStatus.VerifyFail,
            Model = Model
        }, OnSuccessed);
    }

    private void OnSave() => SubmitFilesAsync(Client.Apply.SaveApplyAsync);

    private void OnSuccessed()
    {
        UI.CloseDialog();
        OnSuccess?.Invoke(Result.Success(""));
    }

    internal static void ShowMyFlow(IMyFlow flow)
    {
        flow.UI.Show(new DialogOption
        {
            Title = "业务申请【审核】",
            Size = new(960, 600),
            Content = builder => BuildFlowForm(builder, flow)
        });
    }

    private static void BuildFlowForm(RenderTreeBuilder builder, IMyFlow flow)
    {
        builder.Component<ApplyForm>()
               .Set(c => c.InDialog, true)
               .Set(c => c.ReadOnly, true)
               .Set(c => c.PageType, PageType.Verify)
               .Set(c => c.Flow, flow.Flow)
               .Set(c => c.OnSuccess, result => flow.Refresh())
               .Build();
    }
}