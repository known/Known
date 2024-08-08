namespace Known.Repositories;

class SettingRepository
{
    internal static Task<SysSetting> GetUserSettingAsync(Database db, string bizType)
    {
        return db.QueryAsync<SysSetting>(d => d.CreateBy == db.User.UserName && d.BizData == bizType);
    }
}