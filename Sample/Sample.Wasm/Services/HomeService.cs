namespace Sample.Wasm.Services;

public interface IHomeService : IService
{
    Task<HomeInfo> GetHomeAsync();
}

class HomeClient(HttpClient http) : ClientBase(http), IHomeService
{
    public Task<HomeInfo> GetHomeAsync()
    {
        return Http.GetAsync<HomeInfo>("/Home/GetHome");
    }
}