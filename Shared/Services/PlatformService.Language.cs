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

partial class PlatformClient
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria) => Http.QueryAsync<LanguageInfo>("/Platform/QueryLanguages", criteria);
    public Task<Result> FetchLanguagesAsync() => Http.PostAsync("/Platform/FetchLanguages");
    public Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos) => Http.PostAsync("/Platform/DeleteLanguages", infos);
    public Task<Result> ImportLanguagesAsync(UploadInfo<FileFormInfo> info) => Http.PostAsync("/Platform/ImportLanguages", info);
    public Task<Result> SaveLanguageAsync(LanguageInfo info) => Http.PostAsync("/Platform/SaveLanguage", info);
    public Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos) => Http.PostAsync("/Platform/SaveLanguageSettings", infos);
}

partial class PlatformService
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        return Database.Query<SysLanguage>(criteria).ToPageAsync<LanguageInfo>();
    }

    public async Task<Result> FetchLanguagesAsync()
    {
        await MigrateHelper.MigrateLanguagesAsync(Database);
        return Result.Success(Language.FetchSuccess, Language.Datas);
    }

    public async Task<Result> DeleteLanguagesAsync(List<LanguageInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysLanguage>(item.Id);
            }
        });
        if (result.IsValid)
        {
            foreach (var item in infos)
            {
                var info = Language.Datas.FirstOrDefault(m => m.Chinese == item.Chinese);
                if (info != null)
                    Language.Datas.Remove(info);
            }
            result.Data = Language.Datas;
        }
        return result;
    }

    public async Task<Result> ImportLanguagesAsync(UploadInfo<FileFormInfo> info)
    {
        var key = nameof(FileFormInfo.BizType);
        if (info == null || info.Files == null || !info.Files.ContainsKey(key))
            return Result.Error(Language.ImportSelectFile);

        var file = info.Files[key][0];
        using var stream = new MemoryStream(file.Bytes);
        using var excel = ExcelFactory.Create(stream);
        var lines = excel.SheetToDictionaries(0);

        var database = Database;
        var result = await database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var line in lines)
            {
                var model = await db.QueryAsync<SysLanguage>(d => d.Chinese == line.GetValue<string>("简体中文"));
                if (model == null)
                    continue;

                foreach (var item in Language.Settings)
                {
                    if (line.TryGetValue(item.Name, out string value))
                        TypeHelper.SetPropertyValue(model, item.Id, value);
                }
                await db.SaveAsync(model);
            }
        });
        if (result.IsValid)
            Language.Datas = await database.GetLanguagesAsync();
        return result;
    }

    public async Task<Result> SaveLanguageAsync(LanguageInfo info)
    {
        var database = Database;
        var model = await database.QueryAsync<SysLanguage>(d => d.Chinese == info.Chinese);
        model ??= new SysLanguage();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var result = await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
        if (result.IsValid)
            Language.Datas = await database.GetLanguagesAsync();
        return result;
    }

    public Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos)
    {
        Language.Settings = infos;
        return Database.SaveConfigAsync(Constants.KeyLanguage, infos, true);
    }
}