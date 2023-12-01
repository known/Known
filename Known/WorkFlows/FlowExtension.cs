using Known.Blazor;
using Known.Extensions;

namespace Known.WorkFlows;

public static class FlowExtension
{
    public static void SubmitFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormType.Submit, row);
    }

    public static void SubmitFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal("提交", rows, page.Page.Platform.Flow.SubmitFlowAsync);
    }

    public static void RevokeFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : FlowEntity, new() => page.RevokeFlow([row]);

    public static void RevokeFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal("撤回", rows, page.Page.Platform.Flow.RevokeFlowAsync);
    }

    public static void AssignFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal("指派", [row], page.Page.Platform.Flow.AssignFlowAsync);
    }

    public static void VerifyFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormType.Verify, row);
    }

    public static void StopFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal("终止", rows, page.Page.Platform.Flow.StopFlowAsync);
    }

    public static void RepeatFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal("重启", rows, page.Page.Platform.Flow.RepeatFlowAsync);
    }

    private static void ShowFlowModal<TItem>(this PageModel<TItem> page, string name, List<TItem> rows, Func<FlowFormInfo, Task<Result>> action) where TItem : FlowEntity, new()
    {
        var model = new FormModel<FlowFormInfo>(page.UI);
        model.Data = new FlowFormInfo();
        model.Data.BizId = string.Join(",", rows.Select(r => r.Id));
        model.AddRow().AddColumn(c => c.Note, c =>
        {
            c.Name = $"{name}原因";
        });

        var option = new ModalOption
        {
            Title = $"{name}流程",
            Content = builder =>
            {
                page.UI.BuildForm(builder, model);
            }
        };
        option.OnOk = async () =>
        {
            var result = await action?.Invoke(model.Data);
            page.UI.Result(result, async () =>
            {
                await option.OnClose?.Invoke();
                await page.RefreshAsync();
            });
        };
        page.UI.ShowModal(option);
    }
}