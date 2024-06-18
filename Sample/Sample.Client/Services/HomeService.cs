namespace Sample.Client.Services;

class HomeService(HttpClient http) : ClientBase(http), IHomeService
{
    public Task<HomeInfo> GetHomeAsync() => GetAsync<HomeInfo>("Home/GetHome");
}