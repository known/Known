namespace Known.Components;

/// <summary>
/// 移动端分页组件类。
/// </summary>
public partial class AppPager
{
    private int PageCount => (int)Math.Ceiling(TotalCount * 1.0 / Criteria.PageSize);
    private bool CanPrevious => Criteria.PageIndex > 1;
    private bool CanNext => Criteria.PageIndex >= 1 && Criteria.PageIndex < PageCount;

    /// <summary>
    /// 取得或设置总记录大小。
    /// </summary>
    [Parameter] public int TotalCount { get; set; }

    /// <summary>
    /// 取得或设置查询条件对象。
    /// </summary>
    [Parameter] public PagingCriteria Criteria { get; set; }

    /// <summary>
    /// 取得或设置分页改变事件委托。
    /// </summary>
    [Parameter] public Func<Task> OnChanged { get; set; }

    private async Task OnPreviousAsync()
    {
        Criteria.PageIndex--;
        await OnChanged?.Invoke();
    }

    private async Task OnNextAsync()
    {
        Criteria.PageIndex++;
        await OnChanged?.Invoke();
    }
}