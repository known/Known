namespace Known.Reports;

/// <summary>
/// 系统报表服务接口。
/// </summary>
public interface IReportService : IService
{
    /// <summary>
    /// 获取报表块数据。
    /// </summary>
    /// <param name="block">报表块配置。</param>
    /// <returns>数据行列表。</returns>
    Task<List<Dictionary<string, object>>> QueryBlockDataAsync(ReportBlock block);

    /// <summary>
    /// 获取系统报表列表。
    /// </summary>
    /// <param name="sysId">子系统ID。</param>
    /// <returns>报表列表。</returns>
    Task<List<SysReport>> GetReportsAsync(string sysId);

    /// <summary>
    /// 删除系统报表信息。
    /// </summary>
    /// <param name="info">报表信息。</param>
    /// <returns>操作结果。</returns>
    Task<Result> DeleteReportAsync(SysReport info);

    /// <summary>
    /// 保存系统报表信息。
    /// </summary>
    /// <param name="info">报表信息。</param>
    /// <returns>操作结果。</returns>
    Task<Result> SaveReportAsync(SysReport info);
}

[Client]
class ReportClient(HttpClient http) : ClientBase(http), IReportService
{
    public Task<List<Dictionary<string, object>>> QueryBlockDataAsync(ReportBlock block) => Http.PostAsync<ReportBlock, List<Dictionary<string, object>>>("/Report/QueryBlockData", block);
    public Task<List<SysReport>> GetReportsAsync(string sysId) => Http.GetAsync<List<SysReport>>($"/Report/GetReports?sysId={sysId}");
    public Task<Result> DeleteReportAsync(SysReport info) => Http.PostAsync("/Report/DeleteReport", info);
    public Task<Result> SaveReportAsync(SysReport info) => Http.PostAsync("/Report/SaveReport", info);
}

[WebApi, Service]
class ReportService(Context context) : ServiceBase(context), IReportService
{
    /// <inheritdoc />
    public Task<List<Dictionary<string, object>>> QueryBlockDataAsync(ReportBlock block)
    {
        var sourceType = block?.DataSource?.SourceType ?? "Sample";
        return sourceType switch
        {
            "Sample" => Task.FromResult(GetSampleTableData(block)),
            _ => Task.FromResult(GetSampleTableData(block))
        };
    }

    /// <inheritdoc />
    public async Task<List<SysReport>> GetReportsAsync(string sysId)
    {
        var reports = await Database.QueryListAsync<SysReport>(d => d.SysId == sysId);
        return [.. reports.OrderByDescending(r => r.IsFixed).ThenByDescending(r => r.CreateTime)];
    }

    /// <inheritdoc />
    public async Task<Result> DeleteReportAsync(SysReport info)
    {
        if (info.IsFixed && !CurrentUser.IsSystemAdmin())
            return Result.Error("系统固定报表不能删除");

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            await db.DeleteAsync<SysReport>(info.Id);
        });
    }

    /// <inheritdoc />
    public async Task<Result> SaveReportAsync(SysReport info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysReport>(info.Id);

        if (model?.IsFixed == true && !CurrentUser.IsSystemAdmin())
            return Result.Error("系统固定报表不能修改");

        model ??= new SysReport();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var result = await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
        return result;
    }

    private static List<Dictionary<string, object>> GetSampleTableData(ReportBlock block)
    {
        if (block?.BlockType == "Chart")
        {
            var months = new[] { "一月", "二月", "三月", "四月", "五月", "六月" };
            return months.Select((m, i) => new Dictionary<string, object>
            {
                ["month"] = m,
                ["amount"] = new Random().Next(1000, 10000),
                ["value"] = new Random().Next(500, 5000)
            }).ToList();
        }

        return
        [
            new() { ["name"] = "笔记本电脑", ["category"] = "电子产品", ["price"] = 5999, ["stock"] = 120 },
            new() { ["name"] = "机械键盘", ["category"] = "外设", ["price"] = 899, ["stock"] = 350 },
            new() { ["name"] = "4K显示器", ["category"] = "电子产品", ["price"] = 2999, ["stock"] = 80 }
        ];
    }
}