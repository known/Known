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

public class Tree : BaseComponent
{
    [Parameter] public string Id { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul("ztree", attr => attr.Id(Id));
    }
}
