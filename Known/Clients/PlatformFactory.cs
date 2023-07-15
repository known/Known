namespace Known.Clients;

public class PlatformFactory
{
    public PlatformFactory(Context context)
    {
        Module = new ModuleClient(context);
        System = new SystemClient(context);
        Setting = new SettingClient(context);
        Company = new CompanyClient(context);
        Dictionary = new DictionaryClient(context);
        File = new FileClient(context);
        Flow = new FlowClient(context);
        Role = new RoleClient(context);
        User = new UserClient(context);
    }

    public ModuleClient Module { get; }
    public SystemClient System { get; }
    public SettingClient Setting { get; }
    public CompanyClient Company { get; }
    public DictionaryClient Dictionary { get; }
    public FileClient File { get; }
    public FlowClient Flow { get; }
    public RoleClient Role { get; }
    public UserClient User { get; }
}