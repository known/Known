namespace Known.Admin.Services;

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    #region Config
    public async Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    public async Task SaveConfigAsync(Database db, string key, object value)
    {
        var appId = Config.App.Id;
        var data = new Dictionary<string, object>();
        data[nameof(SysConfig.AppId)] = appId;
        data[nameof(SysConfig.ConfigKey)] = key;
        data[nameof(SysConfig.ConfigValue)] = Utils.ToJson(value);
        var scalar = await db.CountAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        if (scalar > 0)
            await db.UpdateAsync(nameof(SysConfig), "AppId,ConfigKey", data);
        else
            await db.InsertAsync(nameof(SysConfig), data);
    }
    #endregion

    #region Company
    private const string KeyCompany = "CompanyInfo";

    public async Task<string> GetCompanyAsync()
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            return await GetCompanyDataAsync(database);
        }
        else
        {
            var json = await GetConfigAsync(database, KeyCompany);
            if (string.IsNullOrEmpty(json))
                json = GetDefaultData(database.User);
            return json;
        }
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var company = await database.QueryAsync<SysCompany>(d => d.Code == CurrentUser.CompNo);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.CompanyData = Utils.ToJson(model);
            await database.SaveAsync(company);
        }
        else
        {
            await SaveConfigAsync(database, KeyCompany, model);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    private static async Task<string> GetCompanyDataAsync(Database db)
    {
        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        if (company == null)
            return GetDefaultData(db.User);

        var model = company.CompanyData;
        if (string.IsNullOrEmpty(model))
        {
            model = Utils.ToJson(new
            {
                company.Code,
                company.Name,
                company.NameEn,
                company.SccNo,
                company.Address,
                company.AddressEn
            });
        }
        return model;
    }

    private static string GetDefaultData(UserInfo user)
    {
        return Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
    }
    #endregion

    #region User
    public Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        return db.QueryAsync<UserInfo>(d => d.UserName == userName);
    }
    #endregion

    #region Import
    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var db = Database;
        var task = await db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                           .OrderByDescending(d => d.CreateTime).FirstAsync();
        var info = new ImportFormInfo { BizId = bizId, BizType = ImportHelper.BizType, IsFinished = true };
        if (task != null)
        {
            switch (task.Status)
            {
                case SysTaskStatus.Pending:
                    info.Message = context.Language["Import.TaskPending"];
                    info.IsFinished = false;
                    break;
                case SysTaskStatus.Running:
                    info.Message = context.Language["Import.TaskRunning"];
                    info.IsFinished = false;
                    break;
                case SysTaskStatus.Failed:
                    info.Message = context.Language["Import.TaskFailed"];
                    info.Error = task.Note;
                    break;
                case SysTaskStatus.Success:
                    info.Message = "";
                    break;
            }
        }
        return info;
    }

    public async Task<byte[]> GetImportRuleAsync(string bizId)
    {
        var db = Database;
        if (bizId.StartsWith("Dictionary"))
        {
            var id = bizId.Split('_')[1];
            var module = await db.QueryByIdAsync<SysModule>(id);
            return GetImportRule(db.Context, module.GetFormFields());
        }

        var columns = ImportHelper.GetImportColumns(db.Context, bizId);
        if (columns == null || columns.Count == 0)
            return [];

        var fields = columns.Select(c => new FormFieldInfo
        {
            Id = c.Id,
            Name = db.Context.Language.GetString(c),
            Required = c.Required,
            Length = GetImportRuleNote(db.Context, c)
        }).ToList();
        return GetImportRule(db.Context, fields);
    }

    public async Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        SysTask task = null;
        var form = info.Model;
        var sysFiles = new List<SysFile>();
        var database = Database;
        var files = info.Files.GetAttachFiles(CurrentUser, "Upload", form);
        var result = await database.TransactionAsync(Language.Upload, async db =>
        {
            sysFiles = await this.AddFilesAsync(db, files, form.BizId, form.BizType);
            if (form.BizType == ImportHelper.BizType)
            {
                task = CreateTask(form);
                task.Target = sysFiles[0].Id;
                if (form.IsAsync)
                    await db.SaveAsync(task);
            }
        });
        result.Data = sysFiles;
        if (result.IsValid && form.BizType == ImportHelper.BizType)
        {
            if (form.IsAsync)
            {
                TaskHelper.NotifyRun(form.BizType);
                result.Message += Language["Import.FileImporting"];
            }
            else if (task != null)
            {
                result = await ImportHelper.ExecuteAsync(database, task);
            }
        }
        return result;
    }

    private static SysTask CreateTask(ImportFormInfo form)
    {
        return new SysTask
        {
            BizId = form.BizId,
            Type = form.BizType,
            Name = form.BizName,
            Target = "",
            Status = SysTaskStatus.Pending
        };
    }

    private static byte[] GetImportRule(Context context, List<FormFieldInfo> fields)
    {
        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", context.Language["Import.TemplateTips"], new StyleInfo { IsBorder = true });
        sheet.MergeCells(0, 0, 1, fields.Count);
        for (int i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var note = !string.IsNullOrWhiteSpace(field.Length) ? $"{field.Length}" : "";
            sheet.SetColumnWidth(i, 13);
            sheet.SetCellValue(1, i, note, new StyleInfo { IsBorder = true, IsTextWrapped = true });
            var fontColor = field.Required ? Color.Red : Color.White;
            sheet.SetCellValue(2, i, field.Name, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    private static string GetImportRuleNote(Context context, ColumnInfo column)
    {
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            var codes = Cache.GetCodes(column.Category);
            return context.Language["Import.TemplateFill"].Replace("{text}", $"{string.Join(",", codes.Select(c => c.Code))}");
        }

        return column.Note;
    }
    #endregion

    #region File
    public Task<List<SysFile>> GetFilesAsync(string bizId)
    {
        return this.GetFilesAsync(Database, bizId);
    }

    public Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return FileService.AddFilesAsync(db, files, bizId, bizType);
    }

    public async Task<Result> DeleteFileAsync(SysFile file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteAsync(file);
        AttachFile.DeleteFile(file);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        return FileService.DeleteFilesAsync(db, bizId, oldFiles);
    }
    #endregion
}