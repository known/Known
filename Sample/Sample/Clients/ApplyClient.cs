namespace Sample.Clients;

public class ApplyClient : ClientBase
{
    public ApplyClient(Context context) : base(context) { }

    //Apply
    public Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria) => Context.QueryAsync<TbApply>("Apply/QueryApplys", criteria);
    public Task<TbApply> GetDefaultApplyAsync(ApplyType bizType) => Context.GetAsync<TbApply>($"Apply/GetDefaultApply?bizType={bizType}");
    public Task<TbApply> GetApplyAsync(string id) => Context.GetAsync<TbApply>($"Apply/GetApply?id={id}");
    public Task<Result> DeleteApplysAsync(List<TbApply> models) => Context.PostAsync("Apply/DeleteApplys", models);
    public Task<Result> SaveApplyAsync(HttpContent content) => Context.PostAsync("Apply/SaveApply", content);
}