using System.Drawing;
using Known.Cells;

namespace Known.Core.Services;

class DataService(Context context) : ServiceBase(context), IDataService
{
    //Config
    public Task<string> GetConfigAsync(string key) => Admin.GetConfigAsync(Database, key);
    public Task SaveConfigAsync(ConfigInfo info) => Admin.SaveConfigAsync(Database, info.Key, info.Value);

    //System
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

    public Task<Result> SaveUserSettingAsync(UserSettingInfo info)
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

    #region File
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
    #endregion
}