namespace Known.Sample.Reports;

/// <summary>
/// 报表服务接口。
/// </summary>
public interface IReportService : IService
{
    /// <summary>
    /// 异步获取报表列表。
    /// </summary>
    /// <param name="bizType">业务类型。</param>
    /// <returns></returns>
    Task<List<SysReport>> GetReportsAsync(string bizType = null);

    /// <summary>
    /// 异步分页查询报表定义。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <param name="bizType">业务类型。</param>
    /// <returns></returns>
    Task<PagingResult<SysReport>> QueryReportsAsync(PagingCriteria criteria, string bizType = null);

    /// <summary>
    /// 异步获取报表定义。
    /// </summary>
    /// <param name="reportId">报表ID。</param>
    /// <returns></returns>
    Task<SysReport> GetReportAsync(string reportId);

    /// <summary>
    /// 异步查询报表数据。
    /// </summary>
    /// <param name="reportId">报表ID。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<Dictionary<string, object>>> QueryReportDataAsync(string reportId, PagingCriteria criteria);

    /// <summary>
    /// 异步预览报表。
    /// </summary>
    /// <param name="report">报表定义。</param>
    /// <returns></returns>
    Task<ReportPreviewInfo> PreviewReportAsync(SysReport report);

    /// <summary>
    /// 异步保存报表。
    /// </summary>
    /// <param name="report">报表定义。</param>
    /// <returns></returns>
    Task<Result> SaveReportAsync(SysReport report);

    /// <summary>
    /// 异步删除报表。
    /// </summary>
    /// <param name="reports">报表列表。</param>
    /// <returns></returns>
    Task<Result> DeleteReportsAsync(List<SysReport> reports);

    /// <summary>
    /// 异步获取报表数据表列表。
    /// </summary>
    /// <returns></returns>
    Task<List<CodeInfo>> GetReportTablesAsync();

    /// <summary>
    /// 异步获取报表字段列表。
    /// </summary>
    /// <param name="sourceType">数据源类型。</param>
    /// <param name="source">数据源。</param>
    /// <returns></returns>
    Task<List<ReportFieldInfo>> GetReportFieldsAsync(string sourceType, string source);
}

[Client]
class ReportClient(HttpClient http) : ClientBase(http), IReportService
{
    public Task<List<SysReport>> GetReportsAsync(string bizType = null) => Http.GetAsync<List<SysReport>>($"/Report/GetReports?bizType={bizType}");
    public Task<PagingResult<SysReport>> QueryReportsAsync(PagingCriteria criteria, string bizType = null) => Http.QueryAsync<SysReport>($"/Report/QueryReports?bizType={bizType}", criteria);
    public Task<SysReport> GetReportAsync(string reportId) => Http.GetAsync<SysReport>($"/Report/GetReport?reportId={reportId}");
    public Task<PagingResult<Dictionary<string, object>>> QueryReportDataAsync(string reportId, PagingCriteria criteria) => Http.QueryAsync<Dictionary<string, object>>($"/Report/QueryReportData?reportId={reportId}", criteria);
    public Task<ReportPreviewInfo> PreviewReportAsync(SysReport report) => Http.PostAsync<SysReport, ReportPreviewInfo>("/Report/PreviewReport", report);
    public Task<Result> SaveReportAsync(SysReport report) => Http.PostAsync("/Report/SaveReport", report);
    public Task<Result> DeleteReportsAsync(List<SysReport> reports) => Http.PostAsync("/Report/DeleteReports", reports);
    public Task<List<CodeInfo>> GetReportTablesAsync() => Http.GetAsync<List<CodeInfo>>("/Report/GetReportTables");
    public Task<List<ReportFieldInfo>> GetReportFieldsAsync(string sourceType, string source) => Http.GetAsync<List<ReportFieldInfo>>($"/Report/GetReportFields?sourceType={sourceType}&source={Uri.EscapeDataString(source ?? string.Empty)}");
}

[WebApi, Service]
class ReportService(Context context) : ServiceBase(context), IReportService
{
    public async Task<List<SysReport>> GetReportsAsync(string bizType = null)
    {
        var reports = await Database.QueryListAsync<SysReport>();
        return [.. reports.Where(r => string.IsNullOrWhiteSpace(bizType) || r.BizType == bizType)
                          .OrderBy(r => r.Category)
                          .ThenBy(r => r.Sort)
                          .ThenBy(r => r.Name)];
    }

    public Task<PagingResult<SysReport>> QueryReportsAsync(PagingCriteria criteria, string bizType = null)
    {
        if (!string.IsNullOrWhiteSpace(bizType))
            criteria.SetQuery(nameof(SysReport.BizType), QueryType.Equal, bizType);
        return Database.QueryPageAsync<SysReport>(criteria);
    }

    public async Task<SysReport> GetReportAsync(string reportId)
    {
        var report = await Database.QueryByIdAsync<SysReport>(reportId);
        report ??= new SysReport();
        report.Fields ??= [];
        return report;
    }

    public async Task<PagingResult<Dictionary<string, object>>> QueryReportDataAsync(string reportId, PagingCriteria criteria)
    {
        var report = await GetReportAsync(reportId);
        if (string.IsNullOrWhiteSpace(report?.Id))
            return new PagingResult<Dictionary<string, object>> { Message = "报表不存在！" };

        using var db = Database;
        var fields = await EnsureFieldsAsync(db, report);
        var sql = await BuildSqlAsync(db, report, fields);
        if (string.IsNullOrWhiteSpace(sql))
            return new PagingResult<Dictionary<string, object>> { Message = "报表数据源未配置！" };

        PrepareCriteria(criteria, report, fields);
        return await db.QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    public async Task<ReportPreviewInfo> PreviewReportAsync(SysReport report)
    {
        using var db = Database;
        report ??= new SysReport();
        report.Fields ??= [];
        var fields = await EnsureFieldsAsync(db, report);
        var sql = await BuildSqlAsync(db, report, fields);
        if (string.IsNullOrWhiteSpace(sql))
            return new ReportPreviewInfo { Sql = sql, Fields = fields };

        var criteria = new PagingCriteria { PageSize = 10, PageIndex = 1, IsPaging = true };
        PrepareCriteria(criteria, report, fields);
        var result = await db.QueryPageAsync<Dictionary<string, object>>(sql, criteria);
        return new ReportPreviewInfo
        {
            Sql = sql,
            Fields = fields,
            Rows = result?.PageData ?? []
        };
    }

    public async Task<Result> SaveReportAsync(SysReport report)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysReport>(report?.Id);
        model ??= new SysReport();
        model.FillModel(report);
        model.Fields ??= [];

        var validFields = model.Fields.Where(f => !string.IsNullOrWhiteSpace(f.Id)).ToList();
        model.Fields = validFields;

        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            var exists = await database.ExistsAsync<SysReport>(d => d.Id != model.Id && d.Code == model.Code && d.BizType == model.BizType);
            if (exists)
                vr.AddError("同业务类型下报表代码已存在！");
            if (string.IsNullOrWhiteSpace(model.Source))
                vr.AddError("数据源不能为空！");
        }
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            report.Id = model.Id;
        }, report);
    }

    public async Task<Result> DeleteReportsAsync(List<SysReport> reports)
    {
        if (reports == null || reports.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in reports)
            {
                await db.DeleteAsync<SysReport>(item.Id);
            }
        });
    }

    public async Task<List<CodeInfo>> GetReportTablesAsync()
    {
        var tables = await Database.GetTableNamesAsync();
        return [.. tables.OrderBy(t => t).Select(t => new CodeInfo(t, t, t, null))];
    }

    public async Task<List<ReportFieldInfo>> GetReportFieldsAsync(string sourceType, string source)
    {
        if (!string.Equals(sourceType, nameof(ReportSourceType.Table), StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(source))
            return [];

        var fields = await Database.GetTableFieldsAsync(source);
        return [.. fields.Select(ReportFieldInfo.FromField)];
    }

    private static void PrepareCriteria(PagingCriteria criteria, SysReport report, List<ReportFieldInfo> fields)
    {
        criteria ??= new PagingCriteria();
        criteria.IsPaging = report.IsPaging;
        criteria.Fields ??= [];
        if (fields != null)
        {
            foreach (var item in fields)
            {
                if (!string.IsNullOrWhiteSpace(item.Id))
                    criteria.Fields[item.Id] = item.Id;
            }
        }

        if ((criteria.OrderBys == null || criteria.OrderBys.Length == 0) && fields != null)
        {
            criteria.OrderBys = [.. fields.Where(f => !string.IsNullOrWhiteSpace(f.DefaultSort))
                                       .Select(f => $"{f.Id} {f.DefaultSort}" )];
        }
    }

    private async Task<List<ReportFieldInfo>> EnsureFieldsAsync(Database db, SysReport report)
    {
        var fields = report.Fields?.Where(f => !string.IsNullOrWhiteSpace(f.Id)).ToList() ?? [];
        if (fields.Count > 0)
            return fields;

        if (string.Equals(report.SourceType, nameof(ReportSourceType.Table), StringComparison.OrdinalIgnoreCase))
        {
            var sourceFields = await db.GetTableFieldsAsync(report.Source);
            return [.. sourceFields.Select(ReportFieldInfo.FromField)];
        }

        if (string.Equals(report.SourceType, nameof(ReportSourceType.Sql), StringComparison.OrdinalIgnoreCase))
        {
            var sql = report.Source?.Trim().TrimEnd(';');
            if (string.IsNullOrWhiteSpace(sql))
                return [];
            //var rows = await db.QueryListAsync<Dictionary<string, object>>(db.Provider.GetTopSql(1, sql));
            var rows = await db.QueryListAsync<Dictionary<string, object>>(sql);
            return [.. rows.FirstOrDefault()?.GetColumns().Select(c => new ReportFieldInfo
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Width = c.Width,
                IsVisible = c.IsVisible,
                IsSort = c.IsSort
            }) ?? []];
        }

        return fields;
    }

    private async Task<string> BuildSqlAsync(Database db, SysReport report, List<ReportFieldInfo> fields)
    {
        if (report == null || string.IsNullOrWhiteSpace(report.Source))
            return string.Empty;

        if (string.Equals(report.SourceType, nameof(ReportSourceType.Sql), StringComparison.OrdinalIgnoreCase))
            return report.Source.Trim().TrimEnd(';');

        var tableName = db.FormatName(report.Source);
        var selectedFields = fields?.Where(f => f.IsVisible).ToList() ?? [];
        if (selectedFields.Count == 0)
            return $"select * from {tableName}";

        var columns = selectedFields.Select(f => BuildSelectColumn(db, f));
        var selectSql = string.Join(", ", columns);
        return $"select {selectSql} from {tableName}";
    }

    private static string BuildSelectColumn(Database db, ReportFieldInfo field)
    {
        var id = db.FormatName(field.Id);
        if (string.IsNullOrWhiteSpace(field.Expression))
            return id;

        var expression = field.Expression.Trim();
        if (expression.Equals(field.Id, StringComparison.OrdinalIgnoreCase) || expression.Equals(id, StringComparison.OrdinalIgnoreCase))
            return id;

        return $"{expression} as {id}";
    }
}