namespace Known.MyModule.Services;

public interface IDemoItemService : IService
{
    Task<PagingResult<TbDemoItem>> QueryItemsAsync(PagingCriteria criteria);
    Task<TbDemoItem> GetItemAsync(string id);
    Task<Result> SaveItemAsync(TbDemoItem info);
    Task<Result> DeleteItemsAsync(List<TbDemoItem> infos);
}

[Client]
class DemoItemClient(HttpClient http) : ClientBase(http), IDemoItemService
{
    public Task<PagingResult<TbDemoItem>> QueryItemsAsync(PagingCriteria criteria)
        => Http.QueryAsync<TbDemoItem>("/DemoItem/QueryItems", criteria);

    public Task<TbDemoItem> GetItemAsync(string id)
        => Http.GetAsync<TbDemoItem>($"/DemoItem/GetItem?id={id}");

    public Task<Result> SaveItemAsync(TbDemoItem info)
        => Http.PostAsync("/DemoItem/SaveItem", info);

    public Task<Result> DeleteItemsAsync(List<TbDemoItem> infos)
        => Http.PostAsync("/DemoItem/DeleteItems", infos);
}

[WebApi, Service]
class DemoItemService(Context context) : ServiceBase(context), IDemoItemService
{
    public Task<PagingResult<TbDemoItem>> QueryItemsAsync(PagingCriteria criteria)
        => Database.QueryPageAsync<TbDemoItem>(criteria);

    public async Task<TbDemoItem> GetItemAsync(string id)
    {
        var info = await Database.QueryByIdAsync<TbDemoItem>(id);
        info ??= new TbDemoItem();
        return info;
    }

    public async Task<Result> SaveItemAsync(TbDemoItem info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<TbDemoItem>(info.Id);
        model ??= new TbDemoItem();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var exists = await database.ExistsAsync<TbDemoItem>(d => d.Id != model.Id && d.Code == model.Code);
        if (exists)
            return Result.Error($"项目编码[{model.Code}]已存在！");

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }

    public async Task<Result> DeleteItemsAsync(List<TbDemoItem> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
                await db.DeleteAsync<TbDemoItem>(item.Id);
        });
    }
}
