namespace Known.Internals;

/// <summary>
/// 重新加载容器组件类。
/// </summary>
public class ReloadContainer : Microsoft.AspNetCore.Components.IComponent
{
    private RenderHandle renderHandle;

    /// <summary>
    /// 取得或设置子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 附加呈现处理者。
    /// </summary>
    /// <param name="renderHandle">呈现处理者。</param>
    public void Attach(RenderHandle renderHandle)
    {
        this.renderHandle = renderHandle;
    }

    /// <inheritdoc />
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (ChildContent != null)
        {
            renderHandle.Render(ChildContent);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 重新加载组件。
    /// </summary>
    public void ReloadPage()
    {
        renderHandle.Render(b => { });
        if (ChildContent != null)
        {
            renderHandle.Render(ChildContent);
        }
    }
}