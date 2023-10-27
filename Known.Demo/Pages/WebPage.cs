namespace Known.Demo.Pages;

class WebPage : PageComponent
{
    private ClientFactory client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}

class WebGridView<TModel> : KDataGrid<TModel>
{
    private ClientFactory client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}

class WebGridView<TModel, TForm> : KDataGrid<TModel, TForm>
    where TModel : EntityBase, new()
    where TForm : BaseForm<TModel>
{
    private ClientFactory client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}

class WebForm<TModel> : BaseForm<TModel>
{
    private ClientFactory client;
    protected ClientFactory Client
    {
        get
        {
            client ??= new ClientFactory(CurrentUser);
            return client;
        }
    }
}