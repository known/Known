using System.Text;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Sample.Reports;

/// <summary>
/// 报表中心页面组件类。
/// </summary>
[Route("/sys/reports")]
[Menu(Constants.System, "报表中心", "bar-chart", 7)]
public partial class ReportList
{
    private IReportService Service;
    private TableModel<Dictionary<string, object>> Table;
    private List<SysReport> Reports = [];
    private List<CodeInfo> ReportItems = [];
    private List<ReportFieldInfo> CurrentFields = [];
    private List<Dictionary<string, object>> CurrentRows = [];
    private KChart chart;

    /// <summary>
    /// 取得报表业务类型，子类可覆写。
    /// </summary>
    protected virtual string BizType => string.Empty;

    /// <summary>
    /// 取得当前选中报表。
    /// </summary>
    protected SysReport CurrentReport { get; private set; }

    /// <summary>
    /// 取得是否可设计报表。
    /// </summary>
    protected bool CanDesign => CurrentUser?.IsAdmin() == true;

    /// <summary>
    /// 取得是否显示图表
    /// </summary>
    protected bool ShowChart => CurrentReport != null && CurrentRows.Count > 0 && CurrentReport.ViewType != nameof(ReportViewType.Table);

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IReportService>();
        Table = new TableModel<Dictionary<string, object>>(this)
        {
            ShowName = false,
            ShowToolbar = true,
            ShowSetting = true,
            EnableFilter = false,
            AdvSearch = true,
            OnQuery = OnQueryReportAsync
        };
        await LoadReportsAsync();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (ShowChart && chart != null)
            await chart.ShowAsync(BuildChartOption());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected Task OnReportClick(CodeInfo item)
    {
        return SelectReportAsync(item?.DataAs<SysReport>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void OnNew(MouseEventArgs e)
    {
        ShowReportForm(new SysReport
        {
            BizType = BizType,
            Sort = Reports.Count + 1,
            SourceType = nameof(ReportSourceType.Table)
        }, "新增报表");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void OnEdit(MouseEventArgs e)
    {
        if (CurrentReport == null)
            return;

        ShowReportForm(CurrentReport.CloneReport(), "编辑报表");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected void OnDelete(MouseEventArgs e)
    {
        if (CurrentReport == null)
            return;

        UI.Confirm("确定要删除当前报表？", async () =>
        {
            var result = await Service.DeleteReportsAsync([CurrentReport]);
            UI.Result(result, async () => await LoadReportsAsync());
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected Task OnRefresh(MouseEventArgs e)
    {
        return Table?.RefreshAsync() ?? Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected Task OnExport(MouseEventArgs e)
    {
        if (CurrentReport == null)
            return Task.CompletedTask;

        return Table.ExportDataAsync(CurrentReport.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected Task OnPrint(MouseEventArgs e)
    {
        if (CurrentReport == null || CurrentRows == null || CurrentRows.Count == 0)
        {
            UI.Info("暂无可打印数据！");
            return Task.CompletedTask;
        }

        return JS.PrintAsync(BuildPrintHtml());
    }

    private async Task LoadReportsAsync(string reportId = null)
    {
        Reports = await Service.GetReportsAsync(BizType) ?? [];
        ReportItems = [.. Reports.Select(r => new CodeInfo(r.Id, r.Name, r))];

        var selected = !string.IsNullOrWhiteSpace(reportId)
                     ? Reports.FirstOrDefault(r => r.Id == reportId)
                     : CurrentReport == null
                     ? Reports.FirstOrDefault()
                     : Reports.FirstOrDefault(r => r.Id == CurrentReport.Id) ?? Reports.FirstOrDefault();

        await SelectReportAsync(selected, false);
        await StateChangedAsync();
    }

    private async Task SelectReportAsync(SysReport report, bool refresh = true)
    {
        CurrentReport = report;
        CurrentRows = [];
        CurrentFields = [];
        if (CurrentReport == null)
        {
            Table?.Clear();
            return;
        }

        CurrentReport = await Service.GetReportAsync(CurrentReport.Id) ?? CurrentReport;
        CurrentFields = CurrentReport.Fields?.Where(f => !string.IsNullOrWhiteSpace(f.Id)).ToList() ?? [];
        if (CurrentFields.Count == 0)
            CurrentFields = await Service.GetReportFieldsAsync(CurrentReport.SourceType, CurrentReport.Source) ?? [];

        ConfigureTable(CurrentReport, CurrentFields);
        if (refresh)
            await Table.RefreshAsync();
    }

    private void ConfigureTable(SysReport report, List<ReportFieldInfo> fields)
    {
        Table.Clear();
        Table.Name = report.Name;
        Table.ShowPager = report.IsPaging;
        Table.ShowToolbar = true;
        Table.ShowSetting = true;
        Table.EnableFilter = false;
        Table.AdvSearch = true;
        Table.OnQuery = OnQueryReportAsync;
        Table.Criteria.PageSize = 20;

        var columns = fields.Select(ToColumn).ToList();
        //Table.AllColumns = columns;
        Table.Columns = columns;
        Table.SetQueryColumns();
    }

    private async Task<PagingResult<Dictionary<string, object>>> OnQueryReportAsync(PagingCriteria criteria)
    {
        if (CurrentReport == null)
            return new PagingResult<Dictionary<string, object>>();

        var result = await Service.QueryReportDataAsync(CurrentReport.Id, criteria);
        CurrentRows = result?.PageData ?? [];

        if ((Table.Columns == null || Table.Columns.Count == 0) && CurrentRows.Count > 0)
        {
            var columns = CurrentRows[0].GetColumns();
            //Table.AllColumns = columns;
            Table.Columns = columns;
            Table.SetQueryColumns();
        }
        await StateChangedAsync();
        return result;
    }

    private object BuildChartOption()
    {
        if (CurrentReport == null || CurrentRows.Count == 0)
            return new { };

        var categoryField = CurrentReport.ChartCategoryField;
        var valueField = CurrentReport.ChartValueField;
        var seriesField = CurrentReport.ChartSeriesField;
        if (string.IsNullOrWhiteSpace(categoryField) || string.IsNullOrWhiteSpace(valueField))
            return new { title = new { text = $"{CurrentReport.Name}（未配置图表字段）", left = "center" } };

        var chartType = (CurrentReport.ChartType ?? nameof(ReportChartType.Bar)).ToLower();
        if (chartType == nameof(ReportChartType.Pie).ToLower())
        {
            var pieData = CurrentRows.GroupBy(r => r.GetValue(categoryField)?.ToString() ?? "未分类")
                                     .Select(g => new
                                     {
                                         name = g.Key,
                                         value = g.Sum(r => Utils.ConvertTo<decimal?>(r.GetValue(valueField)) ?? 0)
                                     }).ToArray();
            return new
            {
                title = new { text = CurrentReport.Name, left = "center" },
                tooltip = new { trigger = "item" },
                legend = new { bottom = 0 },
                series = new[]
                {
                    new
                    {
                        type = "pie",
                        radius = new[] { "38%", "68%" },
                        data = pieData,
                        label = new { formatter = "{b}: {c}" }
                    }
                }
            };
        }

        var categories = CurrentRows.Select(r => r.GetValue(categoryField)?.ToString() ?? string.Empty)
                                    .Distinct()
                                    .ToArray();
        if (!string.IsNullOrWhiteSpace(seriesField))
        {
            var series = CurrentRows.GroupBy(r => r.GetValue(seriesField)?.ToString() ?? "默认系列")
                                    .Select(g => new
                                    {
                                        name = g.Key,
                                        type = chartType,
                                        smooth = chartType == "line",
                                        label = new { show = chartType == "bar", position = "top" },
                                        data = categories.Select(c => g.Where(r => (r.GetValue(categoryField)?.ToString() ?? string.Empty) == c)
                                                                       .Sum(r => Utils.ConvertTo<decimal?>(r.GetValue(valueField)) ?? 0))
                                                         .ToArray()
                                    }).ToArray();
            return new
            {
                title = new { text = CurrentReport.Name, left = "center" },
                tooltip = new { trigger = "axis" },
                legend = new { bottom = 0 },
                grid = new { top = "14%", left = "6%", right = "3%", bottom = "16%" },
                xAxis = new { type = "category", data = categories },
                yAxis = new { type = "value" },
                series
            };
        }

        var data = categories.Select(c => CurrentRows.Where(r => (r.GetValue(categoryField)?.ToString() ?? string.Empty) == c)
                                             .Sum(r => Utils.ConvertTo<decimal?>(r.GetValue(valueField)) ?? 0))
                             .ToArray();
        return new
        {
            title = new { text = CurrentReport.Name, left = "center" },
            tooltip = new { trigger = "axis" },
            grid = new { top = "14%", left = "6%", right = "3%", bottom = "12%" },
            xAxis = new { type = "category", data = categories },
            yAxis = new { type = "value" },
            series = new[]
            {
                new
                {
                    name = valueField,
                    type = chartType,
                    smooth = chartType == "line",
                    label = new { show = chartType == "bar", position = "top" },
                    data
                }
            }
        };
    }

    private void ShowReportForm(SysReport data, string title)
    {
        var model = new FormModel<SysReport>(this)
        {
            Title = title,
            Info = new FormInfo { Width = 1280 },
            Data = data,
            Type = typeof(ReportForm),
            OnSave = Service.SaveReportAsync,
            OnSavedAsync = async report => await LoadReportsAsync(report.Id)
        };
        UI.ShowForm(model);
    }

    private string BuildPrintHtml()
    {
        var sb = new StringBuilder();
        sb.Append("<div style='padding:20px;font-family:Segoe UI,Microsoft YaHei;'>");
        sb.Append($"<h2 style='margin:0 0 8px 0;'>{CurrentReport?.Name}</h2>");
        if (!string.IsNullOrWhiteSpace(CurrentReport?.Description))
            sb.Append($"<div style='margin-bottom:16px;color:#666;'>{CurrentReport.Description}</div>");
        sb.Append("<table style='width:100%;border-collapse:collapse;font-size:13px;'>");
        sb.Append("<thead><tr>");
        foreach (var item in CurrentFields.Where(f => f.IsVisible))
        {
            sb.Append($"<th style='border:1px solid #d9d9d9;padding:8px;background:#f7f9fc;text-align:center;'>{item.Name ?? item.Id}</th>");
        }
        sb.Append("</tr></thead><tbody>");
        foreach (var row in CurrentRows)
        {
            sb.Append("<tr>");
            foreach (var item in CurrentFields.Where(f => f.IsVisible))
            {
                var value = row.GetValue(item.Id);
                var text = value == null ? string.Empty : GetPrintText(item, value);
                sb.Append($"<td style='border:1px solid #d9d9d9;padding:8px;text-align:{GetTextAlign(item)};'>{text}</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table></div>");
        return sb.ToString();
    }

    private string GetPrintText(ReportFieldInfo item, object value)
    {
        if (!string.IsNullOrWhiteSpace(item.Category))
            return Cache.GetCodeName(item.Category, value?.ToString());
        if (item.Type == FieldType.Date)
            return Utils.ConvertTo<DateTime?>(value)?.ToString(Config.DateFormat);
        if (item.Type == FieldType.DateTime)
            return Utils.ConvertTo<DateTime?>(value)?.ToString(Config.DateTimeFormat);
        if (item.Type == FieldType.Switch || item.Type == FieldType.CheckBox)
            return Utils.ConvertTo<bool>(value) ? "是" : "否";
        return value?.ToString();
    }

    private static string GetTextAlign(ReportFieldInfo item)
    {
        return item.Align?.ToLower() switch
        {
            "right" => "right",
            "center" => "center",
            _ => "left"
        };
    }

    private static ColumnInfo ToColumn(ReportFieldInfo item)
    {
        return item.ToColumn();
    }
}

static class ReportPageExtension
{
    internal static SysReport CloneReport(this SysReport report)
    {
        return new SysReport
        {
            Id = report.Id,
            Code = report.Code,
            Name = report.Name,
            BizType = report.BizType,
            Category = report.Category,
            Icon = report.Icon,
            SourceType = report.SourceType,
            Source = report.Source,
            Sort = report.Sort,
            ViewType = report.ViewType,
            ChartType = report.ChartType,
            ChartCategoryField = report.ChartCategoryField,
            ChartValueField = report.ChartValueField,
            ChartSeriesField = report.ChartSeriesField,
            ChartHeight = report.ChartHeight,
            IsPaging = report.IsPaging,
            Description = report.Description,
            Note = report.Note,
            Fields = [.. report.Fields?.Select(f => new ReportFieldInfo
            {
                Id = f.Id,
                Name = f.Name,
                Expression = f.Expression,
                Type = f.Type,
                Category = f.Category,
                Width = f.Width,
                Align = f.Align,
                IsQuery = f.IsQuery,
                IsVisible = f.IsVisible,
                IsSort = f.IsSort,
                DefaultSort = f.DefaultSort,
                Unit = f.Unit,
                Note = f.Note
            }) ?? []]
        };
    }
}