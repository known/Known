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

public class CheckBox : Field
{
    [Parameter] public string Text { get; set; }

    internal override string GridCellStyle => "check";

    protected override void BuildGridCellText(RenderTreeBuilder builder, object value)
    {
        var check = Utils.ConvertTo<bool>(value);
        builder.Check(attr => attr.Disabled(true).Checked(check));
    }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuildRadio(builder, "checkbox", Text, "True", Value == "True", (isCheck, value) =>
        {
            Value = isCheck ? "True" : "False";
        });
    }
}

public class CheckList : ListField
{
    private readonly Dictionary<string, bool> values = new();

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            values[item.Code] = CheckChecked(item.Code);
            BuildRadio(builder, "checkbox", item.Name, item.Code, values[item.Code], (isCheck, value) =>
            {
                values[value] = isCheck;
                Value = string.Join(",", values.Where(v => v.Value).Select(k => k.Key));
            });
        }
    }

    private bool CheckChecked(string item)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;

        return Value.Split(',').Contains(item);
    }
}

public class RadioList : ListField
{
    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            BuildRadio(builder, "radio", item.Name, item.Code, Value == item.Code);
        }
    }
}
