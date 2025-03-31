namespace Known.Internals;

/// <summary>
/// 标签组件类。
/// </summary>
public partial class DataTabs
{
    private string current;

    /// <summary>
    /// 取得或设置标签配置模型。
    /// </summary>
    [Parameter] public TabModel Model { get; set; }

    /// <summary>
    /// 取得或设置标签改变事件委托。
    /// </summary>
    [Parameter] public Action<string> OnChange { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model.OnStateChanged = StateChanged;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);
            if (current != Model.Current)
            {
                Model.Current = current;
                Model.Change();
            }
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }
}