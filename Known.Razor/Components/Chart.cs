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

public class ChartData
{
    public string Name { get; set; }
    public Dictionary<string, object> Series { get; set; }
}

public class ChartOption
{
    public string Id { get; set; }
    public object Option { get; set; }
}

public class Chart : BaseComponent
{
    [Parameter] public string Id { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public object YAxis { get; set; }
    [Parameter] public object Legend { get; set; }
    [Parameter] public object Tooltip { get; set; }
    [Parameter] public object PlotOptions { get; set; }

    public void ShowLine(ChartData[] datas)
    {
        object xAxis = null;
        object series = null;
        if (datas != null && datas.Length > 0)
        {
            xAxis = new { categories = datas[0].Series.Keys.ToArray() };
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Values.ToArray(),
                showInLegend = datas.Length > 1
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            title = new { text = Title },
            xAxis,
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    public void ShowBar(ChartData[] datas)
    {
        object xAxis = null;
        object series = null;
        if (datas != null && datas.Length > 0)
        {
            xAxis = new { categories = datas[0].Series.Keys.ToArray() };
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Values.ToArray()
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            chart = new { type = "column" },
            title = new { text = Title },
            xAxis,
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    public void ShowPie(ChartData[] datas)
    {
        object series = null;
        if (datas != null && datas.Length > 0)
        {
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Select(s => new { name = s.Key, y = s.Value }).ToArray()
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            chart = new { type = "pie" },
            title = new { text = Title },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { pointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" },
            plotOptions = PlotOptions ?? new { },
            series
        };

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("chart", attr => attr.Id(Id));
    }
}
