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

public class Topbar : AppComponent
{
    [Parameter] public bool ShowBack { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public MenuItem Tool { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("topbar", attr =>
        {
            if (ShowBack)
            {
                builder.Icon("back fa fa-chevron-left", attr =>
                {
                    attr.OnClick(Callback(e => UI.Back()));
                });
                BuildTitle(builder, "back-title");
            }
            else
            {
                BuildTitle(builder);
            }
            BuildTool(builder);
        });
    }

    private void BuildTitle(RenderTreeBuilder builder, string style = "title")
    {
        builder.Span(style, Title);
    }

    private void BuildTool(RenderTreeBuilder builder)
    {
        if (Tool == null)
            return;

        builder.Span($"tool {Tool.Icon}", attr =>
        {
            attr.OnClick(Callback(e => Tool.Action()));
            if (!string.IsNullOrWhiteSpace(Tool.Name))
            {
                builder.Text(Tool.Name);
            }
        });
    }
}
