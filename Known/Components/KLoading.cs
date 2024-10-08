namespace Known.Components;

/// <summary>
/// 内容加载中组件类。
/// </summary>
public class KLoading : BaseComponent
{
    private bool spinning = true;

    /// <summary>
    /// 取得或设置是否是页面。
    /// </summary>
    [Parameter] public bool IsPage { get; set; }

    /// <summary>
    /// 取得或设置组件内部内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 组件呈现后执行的方法。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        spinning = false;
        if (IsPage)
            await StateChangedAsync();
    }

    /// <summary>
    /// 构建旋转组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildSpin(builder, new SpinModel
        {
            Spinning = spinning,
            Content = ChildContent
        });
    }
}