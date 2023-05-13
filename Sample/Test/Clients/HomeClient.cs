namespace Test.Clients;

public class HomeClient : BaseClient
{
    public HomeClient(Context context) : base(context) { }

    public Task<HomeInfo> GetHomeAsync() => Context.GetAsync<HomeInfo>("Home/GetHome");
}