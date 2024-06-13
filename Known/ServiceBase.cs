namespace Known;

public interface IService
{
    string Token { get; set; }
    Context Context { get; set; }
}

public abstract class ServiceBase(Context context) : IService
{
    public string Token { get; set; }
    public Context Context { get; set; } = context;
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
}