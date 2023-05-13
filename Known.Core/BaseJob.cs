namespace Known.Core;

public abstract class BaseJob
{
    protected BaseJob(Context context)
    {
        Context = context;
    }

    public Context Context { get; }
    protected static AppInfo App => KCConfig.App;
    protected UserInfo CurrentUser => Context.CurrentUser;

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