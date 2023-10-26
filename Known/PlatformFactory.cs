namespace Known;

public class PlatformFactory
{
    public PlatformFactory(UserInfo user)
    {
        Module = new ModuleService { CurrentUser = user };
        System = new SystemService { CurrentUser = user };
        Setting = new SettingService{ CurrentUser = user };
        Company = new CompanyService{ CurrentUser = user };
        Dictionary = new DictionaryService{ CurrentUser = user };
        File = new FileService{ CurrentUser = user };
        Flow = new FlowService{ CurrentUser = user };
        Role = new RoleService{ CurrentUser = user };
        User = new UserService{ CurrentUser = user };
    }

    public ModuleService Module { get; }
    public SystemService System { get; }
    public SettingService Setting { get; }
    public CompanyService Company { get; }
    public DictionaryService Dictionary { get; }
    public FileService File { get; }
    public FlowService Flow { get; }
    public RoleService Role { get; }
    public UserService User { get; }
}