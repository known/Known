namespace Sample.Clients;

public class ClientFactory
{
    public ClientFactory(Context context)
    {
        Home = new HomeClient(context);
        Apply = new ApplyClient(context);
    }

    public HomeClient Home { get; }
    public ApplyClient Apply { get; }
}