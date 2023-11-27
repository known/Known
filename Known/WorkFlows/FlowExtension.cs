using Known.Blazor;

namespace Known.WorkFlows;

public static class FlowExtension
{
    public static void SubmitFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : class, new()
    {
        page.ViewForm(FormType.Submit, row);
    }

    public static void RevokeFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : class, new()
    {
        //撤回原因
    }

    public static void VerifyFlow<TItem>(this PageModel<TItem> page, TItem row) where TItem : class, new()
    {
        page.ViewForm(FormType.Verify, row);
    }

    public static void StopFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : class, new()
    {
        //终止原因
    }

    public static void RepeatFlow<TItem>(this PageModel<TItem> page, List<TItem> rows) where TItem : class, new()
    {
        //重启原因
    }
}