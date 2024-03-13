using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KChart : BaseComponent
{
    [Parameter] public object YAxis { get; set; }
    [Parameter] public object Legend { get; set; }
    [Parameter] public object Tooltip { get; set; }
    [Parameter] public object PlotOptions { get; set; }

    public Task ShowLineAsync(string title, ChartDataInfo[] datas)
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
            title = new { text = title },
            xAxis,
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return JS.ShowChartAsync(Id, option);
    }

    public Task ShowBarAsync(string title, ChartDataInfo[] datas)
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
            chart = new { type = "column", backgroundColor = "rgba(0,0,0,0)" },
            title = new { text = title },
            xAxis,
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return JS.ShowChartAsync(Id, option);
    }

    public Task ShowPieAsync(string title, ChartDataInfo[] datas)
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
            chart = new { type = "pie", backgroundColor = "rgba(0,0,0,0)" },
            title = new { text = title },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { pointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return JS.ShowChartAsync(Id, option);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(Id).Class("chart").Close();
    }
}