namespace Known.Services;

/// <summary>
/// 系统报表服务接口。
/// </summary>
public interface IReportService : IService
{
    /// <summary>
    /// 获取系统报表列表。
    /// </summary>
    /// <param name="sysId">系统ID。</param>
    /// <returns></returns>
    Task<List<SysReport>> GetReportsAsync(string sysId);

    /// <summary>
    /// 删除系统报表信息。
    /// </summary>
    /// <param name="info">报表信息。</param>
    /// <returns></returns>
    Task<Result> DeleteReportAsync(SysReport info);

    /// <summary>
    /// 保存系统报表信息。
    /// </summary>
    /// <param name="info">报表信息。</param>
    /// <returns></returns>
    Task<Result> SaveReportAsync(SysReport info);
}

[Client]
class ReportClient(HttpClient http) : ClientBase(http), IReportService
{
    public Task<List<SysReport>> GetReportsAsync(string sysId) => Http.GetAsync<List<SysReport>>($"/Report/GetReports?sysId={sysId}");
    public Task<Result> DeleteReportAsync(SysReport info) => Http.PostAsync("/Report/DeleteReport", info);
    public Task<Result> SaveReportAsync(SysReport info) => Http.PostAsync("/Report/SaveReport", info);
}

[WebApi, Service]
class ReportService(Context context) : ServiceBase(context), IReportService
{
    public Task<List<SysReport>> GetReportsAsync(string sysId)
    {
        return Database.QueryListAsync<SysReport>(d => d.SysId == sysId);
    }

    public Task<Result> DeleteReportAsync(SysReport info)
    {
        return Database.TransactionAsync(Language.Delete, async db =>
        {
            await db.DeleteAsync<SysReport>(info.Id);
        });
    }

    public async Task<Result> SaveReportAsync(SysReport info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysReport>(info.Id);
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
}