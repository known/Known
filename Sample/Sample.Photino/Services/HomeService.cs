namespace Sample.Photino.Services;

public interface IHomeService : IService
{
    Task<HomeInfo> GetHomeAsync();
}

class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public Task<HomeInfo> GetHomeAsync()
    {
        return Task.FromResult(new HomeInfo());
    }
}