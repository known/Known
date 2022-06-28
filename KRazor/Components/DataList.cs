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

public class DataList<TItem> : DataComponent<TItem>
{
    [Parameter] public bool ShowStyleButton { get; set; } = false;
    [Parameter] public string ListStyle { get; set; } = "squared";
    [Parameter] public string ItemStyle { get; set; }

    protected override string ContainerStyle => "list-view";
    protected override string ContentStyle => $"list {ListStyle}";

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Data == null || Data.Count == 0)
        {
            BuildEmpty(builder);
        }
        else
        {
            BuildButtons(builder);
            BuildData(builder);
        }
    }

    private void BuildButtons(RenderTreeBuilder builder)
    {
        if (!ShowStyleButton)
            return;

        builder.Div("list-btns", attr =>
        {
            builder.Icon("fa fa-bars " + Active("lists"), attr => {
                attr.Title("列表显示")
                    .OnClick(Callback(e => ListStyle = "lists"));
            });
            builder.Icon("fa fa-th-large " + Active("squared"), attr => {
                attr.Title("宫格显示")
                    .OnClick(Callback(e => ListStyle = "squared"));
            });
        });
    }

    private void BuildData(RenderTreeBuilder builder)
    {
        if (Style == "list")
        {
            builder.Div("list-header", attr =>
            {
                builder.Fragment(HeadTemplate);
            });
        }
        foreach (TItem item in Data)
        {
            builder.Div($"list-item {ItemStyle}", attr =>
            {
                builder.Fragment(ItemTemplate(item));
            });
        }
    }

    private string Active(string style) => ListStyle == style ? "active" : "";
}
