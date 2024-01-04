using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class DictionaryService : ServiceBase
{
    public Task<Result> RefreshCacheAsync() => RefreshCacheAsync(Database, CurrentUser);

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
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(model);
        await RefreshCacheAsync();
        return Result.Success(Language.Success(Language.Save), model.Id);
    }

    private async Task<Result> RefreshCacheAsync(Database db, UserInfo user)
    {
        if (user == null)
            return Result.Error(Context.Language["Tip.NoLogin"]);

        var entities = await DictionaryRepository.GetDictionarysAsync(db, user.AppId, user.CompNo);
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            if (!string.IsNullOrWhiteSpace(e.Name))
                code = $"{code}-{e.Name}";
            return new CodeInfo(e.Category, code, code, e);
        }).ToList();
        Cache.AttachCodes(codes);
        return Result.Success(Context.Language["Tip.RefreshSuccess"], codes);
    }
}