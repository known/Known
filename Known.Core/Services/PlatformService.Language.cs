namespace Known.Services;

partial class PlatformService
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        return Database.Query<SysLanguage>(criteria).ToPageAsync<LanguageInfo>();
    }

    public async Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysLanguage>(item.Id);
            }
        });
        return result;
    }

    public async Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysLanguage>(info.Id);
        model ??= new SysLanguage();
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