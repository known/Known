namespace Test.Clients;

public class ClientFactory
{
    public ClientFactory(Context context)
    {
        Home = new HomeClient(context);
    }

    public HomeClient Home { get; }
}