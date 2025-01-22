namespace Known.Services;

public partial interface IPlatformService
{
    /// <summary>
    /// 异步分页查询语言信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除语言信息列表。
    /// </summary>
    /// <param name="infos">语言信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos);

    /// <summary>
    /// 异步保存语言信息列表。
    /// </summary>
    /// <param name="info">语言信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveLanguageAsync(LanguageInfo info);
}

partial class PlatformService
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        var datas = AppData.Data.Languages ?? [];
        if (datas.Count == 0)
            datas.AddRange(Language.Items.Select(CreateLanguage));
        if (criteria.HasQuery(nameof(LanguageInfo.Name)))
        {
            var name = criteria.GetQueryValue(nameof(LanguageInfo.Name));
            datas = datas.Where(b => b.Name.Contains(name)).ToList();
        }
        var result = datas.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    public Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        foreach (var info in infos)
        {
            var item = AppData.Data.Languages.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                AppData.Data.Languages.Remove(item);
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    public Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        var item = AppData.Data.Languages.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new LanguageInfo();
            AppData.Data.Languages.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
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
}

partial class PlatformClient
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<LanguageInfo>("/Platform/QueryLanguages", criteria);
    }

    public Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        return Http.PostAsync("/Platform/DeleteLanguages", infos);
    }

    public Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        return Http.PostAsync("/Platform/SaveLanguage", info);
    }
}