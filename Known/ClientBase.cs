namespace Known;

public class ClientBase
{
    protected ClientBase(Context context)
    {
        Context = context;
    }

    protected Context Context { get; }
}