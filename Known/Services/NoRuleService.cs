namespace Known.Services;

/// <summary>
/// 编码规则服务接口。
/// </summary>
public interface INoRuleService : IService
{
    /// <summary>
    /// 异步查询通用编码规则。
    /// </summary>
    /// <param name="criteria">分页条件。</param>
    /// <returns></returns>
    Task<PagingResult<SysNoRule>> QueryNoRulesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除通用编码规则。
    /// </summary>
    /// <param name="infos">规则列表。</param>
    /// <returns></returns>
    Task<Result> DeleteNoRulesAsync(List<SysNoRule> infos);

    /// <summary>
    /// 异步保存通用编码规则。
    /// </summary>
    /// <param name="info">编码规则。</param>
    /// <returns></returns>
    Task<Result> SaveNoRuleAsync(SysNoRule info);
}

[Client]
class NoRuleClient(HttpClient http) : ClientBase(http), INoRuleService
{
    public Task<PagingResult<SysNoRule>> QueryNoRulesAsync(PagingCriteria criteria) => Http.QueryAsync<SysNoRule>("/NoRule/QueryNoRules", criteria);
    public Task<Result> DeleteNoRulesAsync(List<SysNoRule> infos) => Http.PostAsync("/NoRule/DeleteNoRules", infos);
    public Task<Result> SaveNoRuleAsync(SysNoRule info) => Http.PostAsync("/NoRule/SaveNoRule", info);
}

[WebApi, Service]
class NoRuleService(Context context) : ServiceBase(context), INoRuleService
{
    public Task<PagingResult<SysNoRule>> QueryNoRulesAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<SysNoRule>(criteria);
    }

    public async Task<Result> DeleteNoRulesAsync(List<SysNoRule> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysNoRule>(item.Id);
            }
        });
    }

    public async Task<Result> SaveNoRuleAsync(SysNoRule info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysNoRule>(info.Id);
        model ??= new SysNoRule();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }
}