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

public class Button : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Button(Style, attr =>
        {
            attr.Disabled(!Enabled).OnClick(Callback(e => OnButtonClick()));
            if (!string.IsNullOrWhiteSpace(Icon))
            {
                builder.Icon(Icon);
            }
            builder.Span(Text);
        });
    }

    private void OnButtonClick()
    {
        Enabled = false;
        if (OnClick.HasDelegate)
        {
            var task = OnClick.InvokeAsync();
            if (task.IsCompleted)
            {
                Enabled = true;
            }
        }
    }
}
