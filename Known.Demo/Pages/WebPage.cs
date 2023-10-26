namespace Known.Demo.Pages;

public class WebPage : PageComponent
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}

public class WebGridView<TModel> : KDataGrid<TModel>
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}

public class WebGridView<TModel, TForm> : KDataGrid<TModel, TForm>
    where TModel : EntityBase, new()
    where TForm : BaseForm<TModel>
{
    private ClientFactory? client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
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
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}