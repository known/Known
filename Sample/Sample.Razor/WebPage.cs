using Sample.Clients;

namespace Sample.Razor;

public class WebPage : PageComponent
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            Context.Http = Http;
            client ??= new ClientFactory(Context);
            return client;
        }
    }
}

public class WebGridView<TModel> : DataGrid<TModel>
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            Context.Http = Http;
            client ??= new ClientFactory(Context);
            return client;
        }
    }
}

public class WebGridView<TModel, TForm> : DataGrid<TModel, TForm> 
    where TModel : EntityBase, new() 
    where TForm : BaseForm<TModel>
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            Context.Http = Http;
            client ??= new ClientFactory(Context);
            return client;
        }
    }
}

public class WebForm<TModel> : BaseForm<TModel>
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            Context.Http = Http;
            client ??= new ClientFactory(Context);
            return client;
        }
    }
}