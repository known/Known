namespace Known;

public interface IService
{
    Context Context { get; set; }
}

public abstract class ServiceBase(Context context) : IService
{
    public Context Context { get; set; } = context;
    public UserInfo CurrentUser => Context.CurrentUser;
    public Language Language => Context.Language;

    protected virtual Database Database
    {
        get
        {
            var db = Platform.CreateDatabase();
            db.User = CurrentUser;
            db.Context = Context;
            return db;
        }
    }

    private IDataRepository repository;
    protected IDataRepository Repository
    {
        get
        {
            repository ??= Platform.CreateRepository();
            return repository;
        }
    }
}