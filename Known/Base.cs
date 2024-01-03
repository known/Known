namespace Known;

public abstract class ServiceBase
{
    public Context Context { get; set; }
    public UserInfo CurrentUser => Context.CurrentUser;
    public Language Language => Context.Language;

    private PlatformService platform;
    protected PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(Context);
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
            database.Context = Context;
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
}