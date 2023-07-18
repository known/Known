namespace Known.Core;

public abstract class BaseService : ServiceBase
{
    protected BaseService(Context context)
    {
        Context = context;
    }

    protected Context Context { get; }
    protected UserInfo CurrentUser => Context.CurrentUser;

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
    protected static T GetConfig<T>(Database db, string key)
    {
        var json = PlatformRepository.GetConfig(db, Config.AppId, key);
        return Utils.FromJson<T>(json);
    }

    protected static void SaveConfig(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        PlatformRepository.SaveConfig(db, Config.AppId, key, json);
    }

    protected static AttachFile GetAttachFile(UploadFormInfo info, UserInfo user, string key, string typePath) => GetAttachFile(info, user, key, new FileFormInfo { BizType = typePath });

    internal static AttachFile GetAttachFile(UploadFormInfo info, UserInfo user, string key, FileFormInfo form)
    {
        if (info.Files == null || info.Files.Count == 0)
            return null;

        if (!info.Files.ContainsKey(key))
            return null;

        var file = info.Files[key];
        return new AttachFile(file, user, form.BizType, form.BizPath) { Category2 = form.Category };
    }

    protected static List<AttachFile> GetAttachFiles(UploadFormInfo info, UserInfo user, string key, string typePath) => GetAttachFiles(info, user, key, new FileFormInfo { BizType = typePath });

    internal static List<AttachFile> GetAttachFiles(UploadFormInfo info, UserInfo user, string key, FileFormInfo form)
    {
        if (info.MultiFiles == null || info.MultiFiles.Count == 0)
            return null;

        if (!info.MultiFiles.ContainsKey(key))
            return null;

        var attaches = new List<AttachFile>();
        var files = info.MultiFiles[key];
        foreach (var item in files)
        {
            var attach = new AttachFile(item, user, form.BizType, form.BizPath) { Category2 = form.Category };
            attaches.Add(attach);
        }
        return attaches;
    }
    #endregion
}