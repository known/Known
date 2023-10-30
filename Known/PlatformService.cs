namespace Known;

public class PlatformService
{
    public PlatformService(UserInfo user)
    {
        Module = new ModuleService { CurrentUser = user };
        System = new SystemService { CurrentUser = user };
        Setting = new SettingService { CurrentUser = user };
        Company = new CompanyService { CurrentUser = user };
        Dictionary = new DictionaryService { CurrentUser = user };
        File = new FileService { CurrentUser = user };
        Flow = new FlowService { CurrentUser = user };
        Role = new RoleService { CurrentUser = user };
        User = new UserService { CurrentUser = user };
    }

    internal ModuleService Module { get; }
    internal SystemService System { get; }
    internal SettingService Setting { get; }
    internal CompanyService Company { get; }
    internal DictionaryService Dictionary { get; }
    internal FileService File { get; }
    internal FlowService Flow { get; }
    internal RoleService Role { get; }
    internal UserService User { get; }

    public Task<T> GetCompanyAsync<T>() => Company.GetCompanyAsync<T>();
    public Task<Result> SaveCompanyAsync(object model) => Company.SaveCompanyAsync(model);
    public Task<UserInfo> GetUserAsync(string userName) => User.GetUserAsync(userName);
    public Task<AdminInfo> GetAdminAsync() => User.GetAdminAsync();
}