namespace Known.Components;

/// <summary>
/// 自定义Balzor断线重连和错误UI组件类。
/// </summary>
public class KBlazorUI : ComponentBase
{
    /// <summary>
    /// 取得或设置UI按钮样式CSS类名。
    /// </summary>
    [Parameter] public string ButtonClass { get; set; } = "ant-btn ant-btn-primary";

    /// <summary>
    /// 取得或设置断线显示模板。
    /// </summary>
    [Parameter] public RenderFragment Show { get; set; }

    /// <summary>
    /// 取得或设置断线连接失败模板。
    /// </summary>
    [Parameter] public RenderFragment Failed { get; set; }

    /// <summary>
    /// 取得或设置断线连接拒绝模板。
    /// </summary>
    [Parameter] public RenderFragment Rejected { get; set; }

    /// <summary>
    /// 取得或设置错误提示模板。
    /// </summary>
    [Parameter] public RenderFragment Error { get; set; }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div().Id("components-reconnect-modal").Children(() =>
        {
            builder.Div("mask", "");
            BuildShow(builder);
            BuildFailed(builder);
            BuildRejected(builder);
        }).Close();
        builder.Div().Id("blazor-error-ui").Children(() =>
        {
            builder.Div("mask", "");
            BuildError(builder);
        }).Close();
    }

    private void BuildShow(RenderTreeBuilder builder)
    {
        builder.Div("show", () =>
        {
            if (Show != null)
                builder.Fragment(Show);
            else
                builder.Div("text", "系统正在更新，请稍候...");
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
                builder.Div("text", "系统连接失败，请确认网络，尝试重新连接！");
                builder.Markup($"<a href=\"javascript:window.Blazor.reconnect()\" class=\"{ButtonClass}\">重新连接</a>");
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
                builder.Div("text", "系统更新成功，请重新加载！");
                builder.Markup($"<a href=\"javascript:location.reload()\" class=\"{ButtonClass}\">重新加载</a>");
            }
        });
    }

    private void BuildError(RenderTreeBuilder builder)
    {
        builder.Div("error", () =>
        {
            if (Error != null)
            {
                builder.Fragment(Error);
            }
            else
            {
                builder.Div("text", "抱歉，系统出错了，请重新加载！");
                builder.Markup($"<a href=\"javascript:location.reload()\" class=\"{ButtonClass}\">重新加载</a>");
            }
        });
    }
}