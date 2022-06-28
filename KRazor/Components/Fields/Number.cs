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

public class Number : Input
{
    [Parameter] public string Unit { get; set; }

    internal override string GridCellStyle => "txt-right";

    protected override void BuildChildText(RenderTreeBuilder builder)
    {
        BuildValueUnit(builder, Value, Unit);
    }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuildInput(builder, "number");
        if (!string.IsNullOrWhiteSpace(Unit))
        {
            builder.Span("unit", Unit);
        }
    }

    protected override void BuildGridCellText(RenderTreeBuilder builder, object value)
    {
        var text = FormatValue(value);
        BuildValueUnit(builder, text, Unit);
    }

    private static void BuildValueUnit(RenderTreeBuilder builder, string value, string unit)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            builder.Text($"{value} {unit}");
        }
    }
}
