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

public class ListBox : ListField
{
    private string curItem;

    [Parameter] public Action<CodeInfo> OnItemClick { get; set; }
    [Parameter] public RenderFragment<CodeInfo> ItemTemplace { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        var enabled = Enabled ? "" : " disabled";
        builder.Ul($"list-box{enabled}", attr =>
        {
            if (ListItems != null && ListItems.Length > 0)
            {
                foreach (var item in ListItems)
                {
                    item.IsActive = curItem == item.Code;
                    var active = item.IsActive ? " active" : "";
                    builder.Li($"item{active}", attr =>
                    {
                        if (Enabled)
                        {
                            attr.OnClick(Callback(e => OnClick(item)));
                        }
                        BuildItem(builder, item);
                    });
                }
            }
        });
    }

    private void BuildItem(RenderTreeBuilder builder, CodeInfo item)
    {
        if (ItemTemplace != null)
        {
            builder.Fragment(ItemTemplace, item);
        }
        else
        {
            builder.Text(item.Name);
        }
    }

    private void OnClick(CodeInfo info)
    {
        curItem = info.Code;
        OnItemClick?.Invoke(info);
    }
}

public class Select : ListField
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string EmptyText { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        Input.BuildIcon(builder, Icon);
        builder.Select(attr =>
        {
            attr.Id(Id).Name(Id).Disabled(!Enabled).OnChange(CreateBinder());
            if (!string.IsNullOrWhiteSpace(EmptyText))
            {
                BuildOption(builder, EmptyText, true);
            }
            if (ListItems != null && ListItems.Length > 0)
            {
                foreach (var item in ListItems)
                {
                    BuildOption(builder, item.Name, item.Code == Value);
                }
            }
        });
    }

    private static void BuildOption(RenderTreeBuilder builder, string text, bool selected)
    {
        builder.Option(attr =>
        {
            attr.Selected(selected);
            builder.Text(text);
        });
    }
}
