namespace Known.Blazor;

public abstract class BaseModel(Context context)
{
    public Context Context { get; } = context;
    public IUIService UI => Context?.UI;
    public Language Language => Context?.Language;
    public UserInfo CurrentUser => Context?.CurrentUser;
}