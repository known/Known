namespace Known.Components;

public class KReconnect : ComponentBase
{
    [Parameter] public RenderFragment Show { get; set; }
    [Parameter] public RenderFragment Failed { get; set; }
    [Parameter] public RenderFragment Rejected { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div().Id("components-reconnect-modal").Children(() =>
        {
            builder.Div("mask", "");
            BuildShow(builder);
            BuildFailed(builder);
            BuildRejected(builder);
        }).Close();
    }

    private void BuildShow(RenderTreeBuilder builder)
    {
        builder.Div("show", () =>
        {
            if (Show != null)
            {
                builder.Fragment(Show);
            }
            else
            {
                builder.Span("系统正在更新，请稍候！");
            }
        });
    }

    private void BuildFailed(RenderTreeBuilder builder)
    {
        builder.Div("failed", () =>
        {
            if (Failed != null)
            {
                builder.Fragment(Failed);
            }
            else
            {
                builder.Span("系统连接失败，请确认网络，尝试重新连接！");
                builder.Markup("<a href=\"javascript:window.Blazor.reconnect()\" class=\"ant-btn ant-btn-primary\">重新连接</a>");
            }
        });
    }

    private void BuildRejected(RenderTreeBuilder builder)
    {
        builder.Div("rejected", () =>
        {
            if (Rejected != null)
            {
                builder.Fragment(Rejected);
            }
            else
            {
                builder.Span("系统更新成功，请重新加载！");
                builder.Markup("<a href=\"javascript:location.reload()\" class=\"ant-btn ant-btn-primary\">重新加载</a>");
            }
        });
    }
}