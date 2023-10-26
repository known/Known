namespace Known.Core;

public abstract class BaseJob
{
    protected static AppInfo App => Config.App;
    protected UserInfo CurrentUser { get; }

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