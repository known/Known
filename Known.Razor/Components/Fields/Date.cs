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

public class Date : Input
{
    public Date()
    {
        Day = DateTime.Now;
        Value = Day.ToString(Format);
    }

    [Parameter] public bool IsRangeQuery { get; set; } = true;
    [Parameter] public DateTime Day { get; set; }

    public virtual string Format => "yyyy-MM-dd";
    internal override string GridCellStyle => "txt-center";

    public override object GetValue()
    {
        return Day;
    }

    internal override void BuildQuery(RenderTreeBuilder builder)
    {
        if (IsRangeQuery)
        {
            builder.Component<DateRange>(attr =>
            {
                attr.Add(nameof(DateRange.Id), Id)
                    .Add(nameof(DateRange.Label), Label);
            });
        }
        else
        {
            builder.Component<Date>(attr =>
            {
                attr.Add(nameof(Date.Id), Id)
                    .Add(nameof(Date.Label), Label);
            });
        }
    }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuidDate(builder, Id, Value, v => Day = v);
    }

    protected override void SetInputValue(object value)
    {
        Day = Utils.ConvertTo<DateTime>(value);
        Value = Day.ToString(Format);
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        var date = Utils.ConvertTo<DateTime>(value);
        return date.ToString(Format);
    }
}

public class DateTimeL : Date
{
    public override string Format => "yyyy-MM-dd HH:mm";

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        var value = Day.ToString("yyyy-MM-ddTHH:mm");
        BuidDate(builder, Id, value, v => Day = v, "datetime-local");
    }
}

public class DateRange : Input
{
    private readonly string format = "yyyy-MM-dd";
    private readonly string split = "~";
    private readonly string[] values = new string[] { "", "" };
    private string startId;
    private string endId;

    [Parameter] public DateTime Start { get; set; } = DateTime.Now.AddMonths(-1);
    [Parameter] public DateTime End { get; set; } = DateTime.Now;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        startId = $"L{Id}";
        endId = $"G{Id}";
        SetValue(Start, 0);
        SetValue(End, 1);
    }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        var start = Start.ToString(format);
        var end = End.ToString(format);

        BuidDate(builder, startId, start, v =>
        {
            SetValue(v, 0);
            Start = v;
        });
        builder.Span(attr => builder.Text(split));
        BuidDate(builder, endId, end, v =>
        {
            SetValue(v, 1);
            End = v;
        });
    }

    private void SetValue(DateTime date, int index)
    {
        values[index] = date.ToString(format);
        Value = string.Join(split, values);
    }
}
