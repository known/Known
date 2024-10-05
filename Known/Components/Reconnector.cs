namespace Known.Components;

/// <summary>
/// 自定义Balzor断线重连和错误UI组件类。
/// </summary>
public class Reconnector : ComponentBase
{
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
        builder.Div("show", () => builder.Fragment(Show));
    }

    private void BuildFailed(RenderTreeBuilder builder)
    {
        builder.Div("failed", () => builder.Fragment(Failed));
    }

    private void BuildRejected(RenderTreeBuilder builder)
    {
        builder.Div("rejected", () => builder.Fragment(Rejected));
    }

    private void BuildError(RenderTreeBuilder builder)
    {
        builder.Div("error", () => builder.Fragment(Error));
    }
}