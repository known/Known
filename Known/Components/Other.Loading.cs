using AntDesign;

namespace Known.Components;

/// <summary>
/// 加载中组件类。
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

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Component<Spin>()
               .Set(c => c.Spinning, spinning)
               .Set(c => c.ChildContent, ChildContent)
               .Build();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        spinning = false;
        if (IsPage)
            await InvokeAsync(StateHasChanged);
    }
}