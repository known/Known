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
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class Hidden : Field
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Input(attr => attr.Type("hidden").Name(Id).Value(Value));
    }
}

public class File : Field
{
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public bool Multiple { get; set; }
    [Parameter] public Action<InputFileChangeEventArgs> OnFileChanged { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("file").Id(Id).Name(Id).Value(Value).Placeholder(Placeholder)
                .Add("multiple", Multiple)
                .Add("onchange", Callback(OnFileChanged));
        });
    }
}
