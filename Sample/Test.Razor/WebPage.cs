using Test.Clients;

namespace Test.Razor;

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

public class WebGridView<TModel, TForm> : DataGrid<TModel, TForm> where TModel : EntityBase where TForm : FormComponent
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