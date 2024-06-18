namespace Sample.Client.Services;

//业务申请逻辑服务
class ApplyService(HttpClient http) : ClientBase(http), IApplyService
{
    //Apply
    //列表分页查询
    public Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria)
    {
        return QueryAsync<TbApply>("Apply/QueryApplys", criteria);
    }

    //获取默认业务申请实体
    public Task<TbApply> GetDefaultApplyAsync(ApplyType bizType)
    {
        return GetAsync<TbApply>($"Apply/GetDefaultApply?bizType={bizType}");
    }

    //删除业务申请
    public Task<Result> DeleteApplysAsync(List<TbApply> models)
    {
        return PostAsync<List<TbApply>>("Apply/DeleteApplys", models);
    }

    //保存业务申请
    public Task<Result> SaveApplyAsync(UploadInfo<TbApply> info)
    {
        return PostAsync<UploadInfo<TbApply>>("Apply/SaveApply", info);
    }
}