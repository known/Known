using Known.Razor.Pages;

namespace Sample.Razor.BizApply.Forms;

[Dialog(960, 600)]
class ApplyForm : WebForm<TbApply>
{
    private TbApply? model;

    [Parameter] public PageType PageType { get; set; }

    protected override Task InitFormAsync()
    {
        model = TModel;
        return base.InitFormAsync();
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
                         Height = 200,
                         Placeholder = "请输入申请单内容"
                     })
                     .Build();
            });
            table.Tr(attr => table.Field<Upload>(f => f.BizFile).ColSpan(3).Build());
        });
        builder.FormList<FlowLogGrid>("流程记录", "", attr =>
        {
            attr.Set(c => c.BizId, model?.Id);
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        //审核页面显示通过和退回按钮
        if (PageType == PageType.Verify)
        {
            builder.Button(ToolButton.Pass, Callback(OnPassFlow));
            builder.Button(ToolButton.Return, Callback(OnReturnFlow));
        }

        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
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

    private void OnSuccessed() => OnSuccess?.Invoke(Result.Success(""));
}