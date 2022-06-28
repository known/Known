/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class Timer : BaseComponent
{
    private DateTime time;
    private readonly System.Timers.Timer timer = new(1000);

    [Parameter] public string Format { get; set; } = "yyyy-MM-dd dddd HH:mm:ss";
    [Parameter] public RenderFragment<DateTime> ChildContent { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTime();
        timer.Elapsed += (sender, eventArgs) => OnTimerCallback();
        timer.Start();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ChildContent != null)
        {
            builder.Fragment(ChildContent, time);
        }
        else
        {
            var provider = new System.Globalization.CultureInfo("zh-CN");
            var timeString = DateTime.Now.ToString(Format, provider);
            builder.Span(timeString);
        }
    }

    protected override void Dispose(bool disposing)
    {
        timer.Dispose();
    }

    private void OnTimerCallback()
    {
        _ = InvokeAsync(() =>
        {
            SetTime();
            StateHasChanged();
        });
    }

    private void SetTime()
    {
        time = DateTime.Now;
    }
}
