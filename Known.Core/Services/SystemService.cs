using System.Drawing;
using Known.Cells;

namespace Known.Core.Services;

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    //Config
    public Task<string> GetConfigAsync(string key) => Admin.GetConfigAsync(Database, key);
    public Task SaveConfigAsync(ConfigInfo info) => Admin.SaveConfigAsync(Database, info.Key, info.Value);

    //System
    public async Task<SystemInfo> GetSystemAsync()
    {
        try
        {
            var database = Database;
            database.EnableLog = false;
            var info = await Admin.GetSystemAsync(database);
            if (info != null)
            {
                info.ProductKey = null;
                info.UserDefaultPwd = null;
            }
            return info;
        }
        catch
        {
            return null;//系统未安装，返回null
        }
    }

    //Install
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = await GetInstallDataAysnc(false);
        if (Config.App.Connections != null)
        {
            info.Databases = Config.App.Connections.Select(c => new DatabaseInfo
            {
                Name = c.Name,
                Type = c.DatabaseType.ToString(),
                ConnectionString = GetDefaultConnectionString(c)
            }).ToList();
        }
        else
        {
            var db = Database.Create();
            info.Databases = [new DatabaseInfo {
                Name = "Default", Type = db.DatabaseType.ToString(),
                ConnectionString = db.ConnectionString
            }];
        }
        return info;
    }

    private static string GetDefaultConnectionString(ConnectionInfo info)
    {
        switch (info.DatabaseType)
        {
            case DatabaseType.Access:
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password=xxx";
            case DatabaseType.SQLite:
                return "Data Source=..\\Sample.db";
            case DatabaseType.SqlServer:
                return "Data Source=localhost;Initial Catalog=Sample;User Id=xxx;Password=xxx;";
            case DatabaseType.Oracle:
                return "Data Source=localhost:1521/orcl;User Id=xxx;Password=xxx;";
            case DatabaseType.MySql:
                return "Data Source=localhost;port=3306;Initial Catalog=Sample;user id=xxx;password=xxx;Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
            case DatabaseType.PgSql:
                return "Host=localhost;Port=5432;Database=Sample;Username=xxx;Password=xxx;";
            case DatabaseType.DM:
                return "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;";
            default:
                return string.Empty;
        }
    }

    public async Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        try
        {
            AppHelper.SetConnection([info]);
            var db = Database.Create(info.Name);
            await db.OpenAsync();
            return Result.Success(Language["Tip.ConnectSuccess"]);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.InstallRequired"]);

        if (info.AdminPassword != info.Password1)
            return Result.Error(Language["Tip.PwdNotEqual"]);

        Console.WriteLine("Known Install");
        Console.WriteLine($"{info.CompNo}-{info.CompName}");
        AppHelper.SetConnection(info.Databases);
        var database = GetDatabase(info);
        await Admin.InitializeTableAsync(database);
        Console.WriteLine("Module is installing...");
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            await Admin.SaveInstallAsync(db, info);
        });
        if (result.IsValid)
        {
            AppHelper.SaveProductKey(info.ProductKey);
            result.Data = await GetInstallDataAysnc(true);
        }
        Console.WriteLine("Module is installed.");
        return result;
    }

    private Database GetDatabase(InstallInfo info)
    {
        var db = Database.Create();
        db.Context = Context;
        db.User = new UserInfo
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            UserName = info.AdminName.ToLower(),
            Name = info.AdminName
        };
        return db;
    }

    private async Task<InstallInfo> GetInstallDataAysnc(bool isCheck)
    {
        var app = Config.App;
        var info = new InstallInfo
        {
            AppName = app.Name,
            ProductId = app.ProductId,
            ProductKey = AppHelper.GetProductKey(),
            AdminName = Constants.SysUserName
        };
        if (isCheck)
            await Admin.CheckKeyAsync(Database);
        return info;
    }

    //System
    public async Task<SystemDataInfo> GetSystemDataAsync()
    {
        var info = await Admin.GetSystemAsync(Database);
        return new SystemDataInfo
        {
            System = info,
            Version = Config.Version,
            RunTime = Utils.Round((DateTime.Now - Config.StartTime).TotalHours, 2)
        };
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var result = await Admin.SaveCompanyDataAsync(database, CurrentUser.CompNo, info);
            if (!result.IsValid)
                return result;
        }
        else
        {
            await Admin.SaveSystemAsync(database, info);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        var database = Database;
        AppHelper.SaveProductKey(info.ProductKey);
        await Admin.SaveSystemAsync(database, info);
        return await Admin.CheckKeyAsync(database);
    }

    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Admin.AddLogAsync(Database, log);
    }

    #region Setting
    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await Admin.GetUserSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> DeleteUserSettingAsync(string bizType)
    {
        var database = Database;
        var setting = await Admin.GetUserSettingAsync(database, bizType);
        if (setting != null)
            await Admin.DeleteSettingAsync(database, setting.Id);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task<Result> SaveUserSettingInfoAsync(UserSettingInfo info)
    {
        return SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Constant.UserSetting,
            BizData = info
        });
    }

    public async Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        var database = Database;
        var setting = await Admin.GetUserSettingAsync(database, info.BizType);
        setting ??= new SettingInfo();
        setting.BizType = info.BizType;
        setting.BizData = Utils.ToJson(info.BizData);
        await Admin.SaveSettingAsync(database, setting);
        return Result.Success(Language.Success(Language.Save));
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
            var json = await Admin.GetConfigAsync(database, KeyCompany);
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
            var result = await Admin.SaveCompanyDataAsync(database, CurrentUser.CompNo, model);
            if (!result.IsValid)
                return result;
        }
        else
        {
            await Admin.SaveConfigAsync(database, KeyCompany, model);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    private async Task<string> GetCompanyDataAsync(Database db)
    {
        var data = await Admin.GetCompanyDataAsync(db, db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return data;

        return GetDefaultData(db.User);
    }

    private static string GetDefaultData(UserInfo user)
    {
        return Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
    }

    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        var db = Database;
        var sql = $@"select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.CompNo=@CompNo and a.UserName<>'admin'";
        criteria.Fields[nameof(UserInfo.Name)] = "a.Name";
        return await db.QueryPageAsync<UserInfo>(sql, criteria);
    }
    #endregion

    #region Import
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Admin.GetFilesAsync(Database, bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Admin.DeleteFileAsync(Database, file.Id);
        AttachFile.DeleteFile(file.Path);
        return Result.Success(Language.Success(Language.Delete));
    }

    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var db = Database;
        var task = await Admin.GetTaskAsync(db, bizId);
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

    public Task<byte[]> GetImportRuleAsync(string bizId)
    {
        byte[] data = null;
        var db = Database;
        if (bizId.StartsWith("Dictionary"))
        {
            var id = bizId.Split('_')[1];
            var module = DataHelper.GetModule(id);
            data = GetImportRule(db.Context, module.GetFormFields());
        }
        else
        {
            var columns = ImportHelper.GetImportColumns(db.Context, bizId);
            if (columns != null && columns.Count > 0)
            {
                var fields = columns.Select(c => new FormFieldInfo
                {
                    Id = c.Id,
                    Name = db.Context.Language.GetString(c),
                    Required = c.Required,
                    Length = GetImportRuleNote(db.Context, c)
                }).ToList();
                data = GetImportRule(db.Context, fields);
            }
        }
        return Task.FromResult(data);
    }

    public async Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        TaskInfo task = null;
        var form = info.Model;
        var sysFiles = new List<AttachInfo>();
        var database = Database;
        var files = info.Files.GetAttachFiles(CurrentUser, "Upload", form);
        var result = await database.TransactionAsync(Language.Upload, async db =>
        {
            sysFiles = await Admin.AddFilesAsync(db, files, form.BizId, form.BizType);
            if (form.BizType == ImportHelper.BizType)
            {
                task = CreateTask(form);
                task.Target = sysFiles[0].Id;
                task.File = sysFiles[0];
                if (form.IsAsync)
                    await Admin.CreateTaskAsync(db, task);
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

    private static TaskInfo CreateTask(ImportFormInfo form)
    {
        return new TaskInfo
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
}