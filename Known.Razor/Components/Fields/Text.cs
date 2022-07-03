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

public class Password : Input
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "password", Placeholder);
    }
}

public class Text : Input
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "text", Placeholder);
    }
}

public class TextArea : Field
{
    [Parameter] public string Placeholder { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        builder.TextArea(attr =>
        {
            attr.Id(Id).Name(Id).Value(Value).Disabled(!Enabled)
                .Placeholder(Placeholder)
                .OnChange(CreateBinder());
        });
    }
}
