namespace Known.Admin.Services;

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
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
            var json = await ConfigHelper.GetConfigAsync(database, KeyCompany);
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
            await ConfigHelper.SaveConfigAsync(database, KeyCompany, model);
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
        return ImportHelper.GetImport(Context, bizId, task);
    }

    public async Task<byte[]> GetImportRuleAsync(string bizId)
    {
        return await ImportHelper.GetImportRuleAsync(Database, bizId);
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
                task = ImportHelper.CreateTask(form);
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