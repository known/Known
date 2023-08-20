namespace Known.Razor;

partial class UIService
{
    public Task ShowFlowChart(FlowInfo info) => InvokeAsync<FlowInfo>("KRazor.showFlow", info);

    public void SubmitFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("提交流程", new FlowFormOption
        {
            UserLabel = "提交给",
            NoteLabel = "备注",
            ConfirmText = "确定要提交？",
            UserRole = model.UserRole,
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.SubmitFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    public void RevokeFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("撤回流程", new FlowFormOption
        {
            Size = new Size(400, 210),
            NoteLabel = "撤回原因",
            NoteRequired = true,
            ConfirmText = "确定要撤回？",
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.RevokeFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    public void AssignFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("指派流程", new FlowFormOption
        {
            UserLabel = "指派给",
            NoteLabel = "备注",
            ConfirmText = "确定要指派？",
            UserRole = model.UserRole,
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.AssignFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    public void VerifyFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("审核流程", new FlowFormOption
        {
            //Size = new Size(400, 210),
            Status = model.BizStatus,
            NoteLabel = model.BizStatus == FlowStatus.VerifyFail ? "退回原因" : "备注",
            NoteRequired = model.BizStatus == FlowStatus.VerifyFail,
            ConfirmText = $"确定要{model.BizStatus}？",
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.VerifyFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    public void RepeatFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("重启流程", new FlowFormOption
        {
            Size = new Size(400, 210),
            NoteLabel = "重启原因",
            NoteRequired = true,
            ConfirmText = "确定要重启？",
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.RepeatFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    public void StopFlow(FlowClient client, FlowFormInfo model, Action onSuccess = null)
    {
        Show("终止流程", new FlowFormOption
        {
            Size = new Size(400, 210),
            NoteLabel = "终止原因",
            NoteRequired = true,
            ConfirmText = "确定要终止？",
            Model = model,
            OnConfirm = async info =>
            {
                var result = await client.StopFlowAsync(info);
                ResultFlow(result, onSuccess);
            }
        });
    }

    private void Show(string title, FlowFormOption option)
    {
        var size = option.Size ?? new Size(400, 260);
        Show<FlowForm>(title, size, action: attr =>
        {
            attr.Set(c => c.InDialog, true)
                .Set(c => c.Option, option)
                .Set(c => c.Model, option.Model);
        });
    }

    private void ResultFlow(Result result, Action onSuccess)
    {
        Result(result, () =>
        {
            CloseDialog();
            onSuccess?.Invoke();
        });
    }
}