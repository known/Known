namespace Known.Clients;

public class DictionaryClient : BaseClient
{
    public DictionaryClient(Context context) : base(context) { }

    public Task<Result> RefreshCacheAsync() => Context.PostAsync("Dictionary/RefreshCache");
    public Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria) => Context.QueryAsync<SysDictionary>("Dictionary/QueryDictionarys", criteria);
    public Task<Result> DeleteDictionarysAsync(List<SysDictionary> models) => Context.PostAsync("Dictionary/DeleteDictionarys", models);
    public Task<Result> SaveDictionaryAsync(object model) => Context.PostAsync("Dictionary/SaveDictionary", model);
}