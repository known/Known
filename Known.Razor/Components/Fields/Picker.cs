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

using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public interface IPicker
{
    string Title { get; }
    Size Size { get; }
    RenderFragment Content { get; }
}

public class Picker : Field
{
    [Parameter] public IPicker Pick { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(true)
                .Value(Value).Required(Required)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder());
        });

        if (Enabled)
        {
            builder.Icon("fa fa-ellipsis-h", attr =>
            {
                attr.OnClick(Callback(e => ShowPicker()));
            });
        }
    }

    private void ShowPicker()
    {
        UI.Show(new DialogOption
        {
            Title = Pick.Title,
            Size = Pick.Size,
            Content = Pick.Content
        });
    }
}
