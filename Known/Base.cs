using Known.Repositories;

namespace Known;

public class ModelBase { }

public abstract class ServiceBase
{
    public UserInfo CurrentUser { get; set; }

    private PlatformService platform;
    protected PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(CurrentUser);
            return platform;
        }
    }

    private Database database;
    protected virtual Database Database
    {
        get
        {
            database ??= new Database();
            database.User = CurrentUser;
            return database;
        }
        set { database = value; }
    }

    public static string GetMaxFormNo(string prefix, string maxNo)
    {
        var lastNo = maxNo.Replace(prefix, "");
        var length = lastNo.Length;
        lastNo = lastNo.TrimStart('0');
        var no = string.IsNullOrWhiteSpace(lastNo) ? 0 : int.Parse(lastNo);
        return string.Format("{0}{1:D" + length + "}", prefix, no + 1);
    }

    #region Protected
    protected static async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await PlatformRepository.GetConfigAsync(db, Config.App.Id, key);
        return Utils.FromJson<T>(json);
    }

    protected static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        await PlatformRepository.SaveConfigAsync(db, Config.App.Id, key, json);
    }
    #endregion
}