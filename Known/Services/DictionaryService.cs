using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class DictionaryService : BaseService
{
    public Task<Result> RefreshCacheAsync() => RefreshCacheAsync(Database, CurrentUser);

    internal static async Task<Result> RefreshCacheAsync(Database db, UserInfo user)
    {
        if (user == null)
            return Result.Error("用户未登录！");

        var entities = await DictionaryRepository.GetDictionarysAsync(db, user.AppId, user.CompNo);
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            if (!string.IsNullOrWhiteSpace(e.Name))
                code = $"{code}-{e.Name}";
            return new CodeInfo(e.Category, code, code, e);
        }).ToList();
        //TODO：缓存数据字典
        //var datas = PlatformHelper.Dictionary?.Invoke(db);
        //if (datas != null && datas.Count > 0)
        //    codes.AddRange(datas);
        Cache.AttachCodes(codes);
        return Result.Success("刷新成功！", codes);
    }

    public Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        return DictionaryRepository.QueryDictionarysAsync(Database, criteria);
    }

    public async Task<Result> DeleteDictionarysAsync(List<SysDictionary> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveDictionaryAsync(SysDictionary model)
    {
        //var entity = await Database.QueryByIdAsync<SysDictionary>((string)model.Id);
        //entity ??= new SysDictionary();
        //entity.FillModel(model);
        var vr = model.Validate();
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(model);
        await RefreshCacheAsync();
        return Result.Success(Language.SaveSuccess, model.Id);
    }
}