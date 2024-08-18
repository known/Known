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
            var db = Database.Create();
            db.User = CurrentUser;
            db.Context = Context;
            return db;
        }
    }

    private IDataRepository repository;
    internal IDataRepository Repository
    {
        get
        {
            repository ??= Database.CreateRepository();
            return repository;
        }
    }
}