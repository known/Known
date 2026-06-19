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
    /// 获取系统实体列表。
    /// </summary>
    /// <returns>实体列表。</returns>
    Task<List<CodeInfo>> GetEntitiesAsync();

    /// <summary>
    /// 获取实体字段列表。
    /// </summary>
    /// <param name="entityName">实体名称。</param>
    /// <returns>字段列表。</returns>
    Task<List<FieldInfo>> GetEntityFieldsAsync(string entityName);

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
    public Task<List<CodeInfo>> GetEntitiesAsync() => Http.GetAsync<List<CodeInfo>>("/Report/GetEntities");
    public Task<List<FieldInfo>> GetEntityFieldsAsync(string entityName) => Http.GetAsync<List<FieldInfo>>($"/Report/GetEntityFields?entityName={entityName}");
    public Task<List<SysReport>> GetReportsAsync(string sysId) => Http.GetAsync<List<SysReport>>($"/Report/GetReports?sysId={sysId}");
    public Task<Result> DeleteReportAsync(SysReport info) => Http.PostAsync("/Report/DeleteReport", info);
    public Task<Result> SaveReportAsync(SysReport info) => Http.PostAsync("/Report/SaveReport", info);
}

[WebApi, Service]
class ReportService(Context context) : ServiceBase(context), IReportService
{
    /// <inheritdoc />
    public async Task<List<Dictionary<string, object>>> QueryBlockDataAsync(ReportBlock block)
    {
        var sourceType = block?.DataSource?.SourceType ?? DataSourceType.Sample;
        switch (sourceType)
        {
            case DataSourceType.Sample:
                return GetSampleTableData(block);
            case DataSourceType.Entity:
                return await QueryEntityBlockDataAsync(block);
            default:
                return GetSampleTableData(block);
        }
    }

    /// <inheritdoc />
    public Task<List<CodeInfo>> GetEntitiesAsync()
    {
        var entities = DbConfig.Models
            .Select(m => new CodeInfo(m.Type.Name, m.Type.Name, m.Type.DisplayName() ?? m.Type.Name, null))
            .OrderBy(e => e.Name)
            .ToList();
        return Task.FromResult(entities);
    }

    /// <inheritdoc />
    public Task<List<FieldInfo>> GetEntityFieldsAsync(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            return Task.FromResult(new List<FieldInfo>());

        var model = DbConfig.Models.FirstOrDefault(m => m.Type.Name == entityName);
        if (model == null)
            return Task.FromResult(new List<FieldInfo>());

        return Task.FromResult(model.Fields);
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

    private async Task<List<Dictionary<string, object>>> QueryEntityBlockDataAsync(ReportBlock block)
    {
        var ds = block?.DataSource;
        if (ds == null || string.IsNullOrWhiteSpace(ds.EntityName))
            return [];

        var entityFields = ds.EntityFields ?? [];
        if (entityFields.Count == 0)
            return [];

        using var db = Database;
        var entityModel = DbConfig.Models.FirstOrDefault(m => m.Type.Name == ds.EntityName);
        if (entityModel == null)
            return [];

        var tableName = db.FormatName(entityModel.Type.TableName());
        var selectParts = new List<string>();
        var groupByParts = new List<string>();

        foreach (var ef in entityFields)
        {
            var fieldName = db.FormatName(ef.FieldName);
            var alias = db.FormatName(ef.DisplayName ?? ef.FieldName);

            if (ef.AggregateType != AggregateType.None)
            {
                var aggFunc = ef.AggregateType switch
                {
                    AggregateType.Count => "count",
                    AggregateType.Sum => "sum",
                    AggregateType.Avg => "avg",
                    AggregateType.Max => "max",
                    AggregateType.Min => "min",
                    _ => "count"
                };
                selectParts.Add($"{aggFunc}({fieldName}) as {alias}");
            }
            else
            {
                selectParts.Add($"{fieldName} as {alias}");
                groupByParts.Add(fieldName);
            }
        }

        var selectSql = string.Join(", ", selectParts);
        var sql = $"select {selectSql} from {tableName}";

        if (groupByParts.Count > 0)
        {
            var groupBySql = string.Join(", ", groupByParts);
            sql += $" group by {groupBySql}";
        }

        return await db.QueryListAsync<Dictionary<string, object>>(sql);
    }

    private static List<Dictionary<string, object>> GetSampleTableData(ReportBlock block)
    {
        if (block?.BlockType == ReportBlockType.Chart)
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
