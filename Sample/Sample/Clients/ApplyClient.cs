namespace Sample.Clients;

public class ApplyClient : ClientBase
{
    public ApplyClient(Context context) : base(context) { }

    //Apply
    public Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria) => Context.QueryAsync<TbApply>("Apply/QueryApplys", criteria);
    public Task<Result> DeleteApplysAsync(List<TbApply> models) => Context.PostAsync("Apply/DeleteApplys", models);
    public Task<Result> SaveApplyAsync(HttpContent content) => Context.PostAsync("Apply/SaveApply", content);
}