namespace Sample.Clients;

public class HomeClient : ClientBase
{
    public HomeClient(Context context) : base(context) { }

    public Task<HomeInfo> GetHomeAsync() => Context.GetAsync<HomeInfo>("Home/GetHome");
}