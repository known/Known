namespace Known;

public class BaseClient
{
    protected BaseClient(Context context)
    {
        Context = context;
    }

    protected Context Context { get; }
}