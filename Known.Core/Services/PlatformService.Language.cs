namespace Known.Services;

partial class PlatformService
{
    public async Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        var datas = await GetLanguagesAsync();
        return datas.ToQueryResult(criteria);
    }

    public async Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var datas = await GetLanguagesAsync();
        foreach (var info in infos)
        {
            var item = datas.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                datas.Remove(item);
        }
        await SaveLanguagesAsync(datas);
        return Result.Success(Language.DeleteSuccess);
    }

    public async Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        var datas = await GetLanguagesAsync();
        var item = datas.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new LanguageInfo();
            datas.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        await SaveLanguagesAsync(datas);
        return Result.Success(Language.SaveSuccess);
    }

    private LanguageInfo CreateLanguage(ActionInfo info)
    {
        return new LanguageInfo
        {
            Id = info.Id,
            Name = info.Name,
            Icon = info.Icon
        };
    }

    private async Task<List<LanguageInfo>> GetLanguagesAsync()
    {
        var datas = await Database.GetConfigAsync<List<LanguageInfo>>(Constant.KeyLanguage, true);
        datas ??= [];
        foreach (var item in Language.Items)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(CreateLanguage(item));
        }
        return datas;
    }

    private Task<Result> SaveLanguagesAsync(List<LanguageInfo> datas)
    {
        return Database.SaveConfigAsync(Constant.KeyLanguage, datas, true);
    }
}