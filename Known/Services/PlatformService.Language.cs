﻿namespace Known.Services;

public partial interface IPlatformService
{
    /// <summary>
    /// 异步分页查询语言信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步提取多语言信息。
    /// </summary>
    /// <returns>提取结果。</returns>
    Task<Result> FetchLanguagesAsync();

    /// <summary>
    /// 异步删除语言信息列表。
    /// </summary>
    /// <param name="infos">语言信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos);

    /// <summary>
    /// 异步导入语言信息数据。
    /// </summary>
    /// <param name="info">导入文件。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportLanguagesAsync(UploadInfo<FileFormInfo> info);

    /// <summary>
    /// 异步保存语言信息列表。
    /// </summary>
    /// <param name="info">语言信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveLanguageAsync(LanguageInfo info);

    /// <summary>
    /// 异步保存语言设置信息列表。
    /// </summary>
    /// <param name="infos">语言设置信息列表。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos);
}

partial class PlatformService
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        var datas = AppData.Data.Languages ?? [];
        var result = datas.ToQueryResult(criteria);
        return Task.FromResult(result);
    }

    public Task<Result> FetchLanguagesAsync()
    {
        return Result.SuccessAsync(Language.FetchSuccess);
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

    public Task<Result> ImportLanguagesAsync(UploadInfo<FileFormInfo> info)
    {
        return Result.SuccessAsync(Language.ImportSuccess);
    }

    public Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        var item = AppData.Data.Languages.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new LanguageInfo();
            AppData.Data.Languages.Add(item);
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class PlatformClient
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<LanguageInfo>("/Platform/QueryLanguages", criteria);
    }

    public Task<Result> FetchLanguagesAsync()
    {
        return Http.PostAsync("/Platform/FetchLanguages");
    }

    public Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        return Http.PostAsync("/Platform/DeleteLanguages", infos);
    }

    public Task<Result> ImportLanguagesAsync(UploadInfo<FileFormInfo> info)
    {
        return Http.PostAsync("/Platform/ImportLanguages", info);
    }

    public Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        return Http.PostAsync("/Platform/SaveLanguage", info);
    }

    public Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos)
    {
        return Http.PostAsync("/Platform/SaveLanguageSettings", infos);
    }
}