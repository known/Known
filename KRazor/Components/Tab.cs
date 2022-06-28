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

public class Tab : BaseComponent
{
    [Parameter] public bool Justified { get; set; }
    //top、left
    [Parameter] public string Position { get; set; } = "top";
    [Parameter] public string Codes { get; set; }
    [Parameter] public string CurItem { get; set; }
    [Parameter] public CodeInfo[] Items { get; set; }
    [Parameter] public EventCallback<CodeInfo> OnChanged { get; set; }

    private CodeInfo[] TabItems { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        TabItems = CodeInfo.GetCodes(Codes, Items);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            var items = TabItems;
            if (items != null && items.Length > 0 && string.IsNullOrWhiteSpace(CurItem))
            {
                OnItemClick(items[0]);
            }
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul($"tab tab-{Position}", attr =>
        {
            var items = TabItems;
            if (items != null && items.Length != 0)
            {
                foreach (var item in items)
                {
                    builder.Li(Active(item.Code), attr =>
                    {
                        attr.OnClick(Callback(e => OnItemClick(item)));
                        if (Justified)
                        {
                            var width = Math.Round(100M / items.Length, 2);
                            attr.Style($"width:{width}%");
                        }
                        builder.Text(item.Name);
                    });
                }
            }
        });
    }

    private void OnItemClick(CodeInfo item)
    {
        CurItem = item.Code;
        if (OnChanged.HasDelegate)
        {
            OnChanged.InvokeAsync(item);
        }
    }

    private string Active(string item) => CurItem == item ? "active" : "";
}
