namespace Sample.Services;

public interface IHomeService : IService
{
    Task<HomeInfo> GetHomeAsync();
}