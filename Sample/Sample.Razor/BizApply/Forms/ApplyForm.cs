using Known.Razor.Pages;

namespace Sample.Razor.BizApply.Forms;

[Dialog(800, 600)]
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
            table.ColGroup(100, null);
            table.Tr(attr => builder.Field<Text>(f => f.BizNo).Enabled(false).Build());
            table.Tr(attr => builder.Field<Text>(f => f.BizTitle).Build());
            table.Tr(attr => builder.Field<TextArea>(f => f.BizContent).Build());
            table.Tr(attr => builder.Field<Text>(f => f.BizFile).Build());
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