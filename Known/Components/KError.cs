namespace Known.Components;

/// <summary>
/// 错误处理组件类。
/// </summary>
public class KError : BaseComponent
{
    /// <summary>
    /// 取得或设置组件子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 取得或设置错误处理委托。
    /// </summary>
    [Parameter] public Action<Exception> OnError { get; set; }

    /// <summary>
    /// 呈现错误组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Cascading(this, ChildContent);
    }

    /// <summary>
    /// 处理异常错误信息。
    /// </summary>
    /// <param name="exception">异常信息。</param>
    /// <returns></returns>
    public void HandleException(Exception exception)
    {
        OnError?.Invoke(exception);
    }
}