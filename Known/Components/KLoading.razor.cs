namespace Known.Components;

/// <summary>
/// 加载中组件类。
/// </summary>
public partial class KLoading
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
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        spinning = false;
        if (IsPage)
            await InvokeAsync(StateHasChanged);
    }
}