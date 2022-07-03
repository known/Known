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

public class Card : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment HeadTemplate { get; set; }
    [Parameter] public RenderFragment BodyTemplate { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("card", attr =>
        {
            BuildHead(builder);
            BuildBody(builder);
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        builder.Div("card-head", attr =>
        {
            if (HeadTemplate != null)
            {
                builder.Fragment(HeadTemplate);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Icon))
                    builder.Icon(Icon);

                builder.Span(Title);
            }
        });
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Div("card-body", attr =>
        {
            builder.Fragment(BodyTemplate);
        });
    }
}
