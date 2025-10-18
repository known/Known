namespace Known.Components;

/// <summary>
/// 时间组件类。
/// </summary>
public class KTimer : BaseComponent
{
    private DateTime time;
    private readonly System.Timers.Timer timer = new(1000);

    /// <summary>
    /// 取得或设置日期时间格式，默认：yyyy-MM-dd HH:mm:ss dddd。
    /// </summary>
    [Parameter] public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss dddd";

    /// <summary>
    /// 取得或设置日期时间自定义模板。
    /// </summary>
    [Parameter] public RenderFragment<DateTime> ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTime();
        timer.Elapsed += (sender, eventArgs) => OnTimerCallback();
        timer.Start();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ChildContent != null)
        {
            builder.Fragment(ChildContent, time);
        }
        else
        {
            var provider = new CultureInfo("zh-CN");
            var timeString = DateTime.Now.ToString(Format, provider);
            builder.Span(timeString);
        }
    }

    /// <inheritdoc />
    protected override Task OnDisposeAsync()
    {
        timer.Dispose();
        return base.OnDisposeAsync();
    }

    private void OnTimerCallback()
    {
        _ = InvokeAsync(() =>
        {
            SetTime();
            StateHasChanged();
        });
    }

    private void SetTime() => time = DateTime.Now;

}