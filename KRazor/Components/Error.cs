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

public class Error : BaseComponent
{
    [Parameter] public string Type { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Message { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var title = Title;
        var message = Message;
        switch (Type)
        {
            case "403":
                title = Language.Error403Title;
                message = Language.Error403Content;
                break;
            case "404":
                title = Language.Error404Title;
                message = Language.Error404Content;
                break;
            case "500":
                title = Language.Error500Title;
                message = Language.Error500Content;
                break;
        }

        builder.Div("error-box", attr =>
        {
            builder.Element("h1", attr => builder.Text(Type));
            builder.Element("h3", attr => builder.Text(title));
            builder.Div(attr => builder.Text(message));
        });
    }
}
