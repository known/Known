namespace Known.Components;

/// <summary>
/// 标签页面组件类。
/// </summary>
public partial class TabPage
{
    private string current;
    private string activeKey;

    /// <summary>
    /// 取得或设置页面名称。
    /// </summary>
    [Parameter] public string PageName { get; set; }

    /// <summary>
    /// 取得或设置查询模型。
    /// </summary>
    [Parameter] public TableModel Query { get; set; }

    /// <summary>
    /// 取得或设置标签页内容模板。
    /// </summary>
    [Parameter] public RenderFragment TabContent { get; set; }

    /// <summary>
    /// 取得或设置标签右侧内容模板。
    /// </summary>
    [Parameter] public RenderFragment TabRight { get; set; }

    /// <summary>
    /// 取得或设置标签底部内容模板。
    /// </summary>
    [Parameter] public RenderFragment TabBottom { get; set; }

    /// <summary>
    /// 取得或设置标签改变事件委托。
    /// </summary>
    [Parameter] public Func<string, Task> OnChange { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (current != activeKey)
        {
            current = activeKey;
            if (OnChange != null)
                await OnChange.Invoke(current);
        }
    }
}