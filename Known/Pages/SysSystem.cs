namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/info")]
public class SysSystem : BaseTabPage
{
    private ISystemService systemService;
    internal SystemInfo Data { get; private set; }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        systemService = await CreateServiceAsync<ISystemService>();
        Data = await systemService.GetSystemAsync();

        Tab.AddTab("SystemInfo", b => b.Component<SysSystemInfo>().Build());
        Tab.AddTab("SecuritySetting", b => b.Component<SysSystemSafe>().Build());
        Tab.AddTab("WeChatSetting", b => b.Component<WeChatSetting>().Build());
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);
}

class SysSystemInfo : BaseForm<SystemInfo>
{
    private ISystemService systemService;
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        systemService = await CreateServiceAsync<ISystemService>();

        Model = new FormModel<SystemInfo>(Context)
        {
            LabelSpan = 4,
            WrapperSpan = 10,
            Data = Parent.Data
        };
        Model.AddRow().AddColumn(nameof(SystemInfo.CompName), $"{Parent.Data.CompNo}-{Parent.Data.CompName}");
        Model.AddRow().AddColumn(nameof(SystemInfo.AppName), b =>
        {
            b.Component<KEditInput>()
             .Set(c => c.Value, Parent.Data.AppName)
             .Set(c => c.OnSave, OnSaveAppName)
             .Build();
        });
        Model.AddRow().AddColumn(nameof(VersionInfo.AppVersion), Config.Version.AppVersion);
        Model.AddRow().AddColumn(nameof(VersionInfo.SoftVersion), Config.Version.SoftVersion);
        Model.AddRow().AddColumn(nameof(VersionInfo.BuildTime), $"{Config.Version.BuildTime:yyyy-MM-dd HH:mm:ss}");
        Model.AddRow().AddColumn(nameof(VersionInfo.FrameVersion), Config.Version.FrameVersion);
        var runTime = Utils.Round((DateTime.Now - Config.StartTime).TotalHours, 2);
        Model.AddRow().AddColumn("RunTime", $"{runTime} H");
        if (!Config.App.IsPlatform && !string.IsNullOrWhiteSpace(Config.App.ProductId))
        {
            Model.AddRow().AddColumn(nameof(SystemInfo.ProductId), Config.App.ProductId);
            Model.AddRow().AddColumn(nameof(SystemInfo.ProductKey), b =>
            {
                b.Component<KEditInput>()
                 .Set(c => c.Value, Parent.Data.ProductKey)
                 .Set(c => c.OnSave, OnSaveProductKey)
                 .Build();
            });
        }
        if (!string.IsNullOrWhiteSpace(Config.App.Copyright))
            Model.AddRow().AddColumn(nameof(AppInfo.Copyright), Config.App.Copyright);
        if (!string.IsNullOrWhiteSpace(Config.App.SoftTerms))
            Model.AddRow().AddColumn(nameof(AppInfo.SoftTerms), Config.App.SoftTerms);
    }

    protected override void BuildForm(RenderTreeBuilder builder) => builder.FormPage(() => base.BuildForm(builder));

    private async void OnSaveAppName(string value)
    {
        Model.Data.AppName = value;
        var result = await systemService.SaveSystemAsync(Model.Data);
        if (result.IsValid)
        {
            CurrentUser.AppName = value;
            App?.StateChanged();
        }
    }

    private async void OnSaveProductKey(string value)
    {
        Model.Data.ProductKey = value;
        await systemService.SaveKeyAsync(Model.Data);
        await StateChangedAsync();
    }
}

class SysSystemSafe : BaseForm<SystemInfo>
{
    private ISystemService systemService;
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        systemService = await CreateServiceAsync<ISystemService>();

        Model = new FormModel<SystemInfo>(Context)
        {
            LabelSpan = 4,
            WrapperSpan = 10,
            Data = Parent.Data
        };
        Model.AddRow().AddColumn(nameof(SystemInfo.UserDefaultPwd), b =>
        {
            b.Component<KEditInput>()
             .Set(c => c.Value, Parent.Data.UserDefaultPwd)
             .Set(c => c.OnSave, OnSaveDefaultPwd)
             .Build();
        });
        Model.AddRow().AddColumn(nameof(SystemInfo.IsLoginCaptcha), b =>
        {
            UI.BuildSwitch(b, new InputModel<bool>
            {
                Value = Parent.Data.IsLoginCaptcha,
                ValueChanged = this.Callback<bool>(OnLoginCaptchaChanged)
            });
        });
    }

    protected override void BuildForm(RenderTreeBuilder builder) => builder.FormPage(() => base.BuildForm(builder));

    private async void OnSaveDefaultPwd(string value)
    {
        Model.Data.UserDefaultPwd = value;
        await systemService.SaveSystemAsync(Model.Data);
    }

    private async void OnLoginCaptchaChanged(bool value)
    {
        Model.Data.IsLoginCaptcha = value;
        await systemService.SaveSystemAsync(Model.Data);
    }
}