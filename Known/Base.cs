using Known.Repositories;

namespace Known;

public class ModelBase { }

public abstract class BaseJob
{
    protected static AppInfo App => Config.App;
    public UserInfo CurrentUser { get; set; }

    private Database database;
    public virtual Database Database
    {
        get
        {
            database ??= new Database();
            database.User = CurrentUser;
            return database;
        }
        set { database = value; }
    }

    public abstract Result Execute();
}

public abstract class BaseService
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
        var json = await PlatformRepository.GetConfigAsync(db, Config.AppId, key);
        return Utils.FromJson<T>(json);
    }

    protected static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        await PlatformRepository.SaveConfigAsync(db, Config.AppId, key, json);
    }

    protected static List<AttachFile> GetAttachFiles(UploadFormInfo info, UserInfo user, string key, string typePath) => GetAttachFiles(info, user, key, new FileFormInfo { BizType = typePath });

    internal static List<AttachFile> GetAttachFiles(UploadFormInfo info, UserInfo user, string key, FileFormInfo form)
    {
        if (info.Files == null || info.Files.Count == 0)
            return null;

        if (!info.Files.ContainsKey(key))
            return null;

        var attaches = new List<AttachFile>();
        var files = info.Files[key];
        foreach (var item in files)
        {
            var attach = new AttachFile(item, user, form.BizType, form.BizPath) { Category2 = form.Category };
            attaches.Add(attach);
        }
        return attaches;
    }
    #endregion
}