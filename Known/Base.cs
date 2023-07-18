namespace Known;

public class ModelBase { }
public class ServiceBase { }

public class ClientBase
{
    protected ClientBase(Context context)
    {
        Context = context;
    }

    protected Context Context { get; }
}