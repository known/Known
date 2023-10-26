namespace Known.Core;

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