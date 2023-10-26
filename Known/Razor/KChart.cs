namespace Known.Razor;

public class ChartOption
{
    public string Id { get; set; }
    public object Option { get; set; }
}

public class KChart : BaseComponent
{
    public object YAxis { get; set; }
    public object Legend { get; set; }
    public object Tooltip { get; set; }
    public object PlotOptions { get; set; }

    public void ShowLine(string title, ChartDataInfo[] datas)
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

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    public void ShowBar(string title, ChartDataInfo[] datas)
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
            title = new { text = title },
            xAxis,
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    public void ShowPie(string title, ChartDataInfo[] datas)
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
            title = new { text = title },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { pointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" },
            plotOptions = PlotOptions ?? new { },
            series
        };

        UI.Show(new ChartOption { Id = Id, Option = option });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Div("chart", attr => attr.Id(Id));
}