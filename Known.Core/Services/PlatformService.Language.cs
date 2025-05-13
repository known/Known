using Known.Cells;

namespace Known.Services;

partial class PlatformService
{
    public Task<PagingResult<LanguageInfo>> QueryLanguagesAsync(PagingCriteria criteria)
    {
        return Database.Query<SysLanguage>(criteria).ToPageAsync<LanguageInfo>();
    }

    public async Task<Result> FetchLanguagesAsync()
    {
        var database = Database;
        var models = await database.Query<SysLanguage>().ToListAsync<LanguageInfo>();
        var datas = new List<SysLanguage>();
        var items = Language.GetDefaultLanguages();
        foreach (var item in items)
        {
            if (models.Exists(m => m.Chinese == item.Chinese))
                continue;

            models.Add(item);
            datas.Add(new SysLanguage { Chinese = item.Chinese });
        }
        await database.InsertListAsync(datas);
        return Result.Success(Language.FetchSuccess, models);
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

        return await Database.TransactionAsync(Language.Import, async db =>
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

    public Task<Result> SaveLanguageSettingsAsync(List<LanguageSettingInfo> infos)
    {
        Language.Settings = infos;
        return Database.SaveConfigAsync(Constant.KeyLanguage, infos, true);
    }
}