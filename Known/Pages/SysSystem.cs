namespace Known.Pages;

[StreamRendering]
[Route("/sys/info")]
public class SysSystem : BaseTabPage
{
    private ISystemService systemService;
    internal SystemDataInfo Data { get; private set; }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        systemService = await CreateServiceAsync<ISystemService>();
        Data = await systemService.GetSystemDataAsync();

        Tab.AddTab("SystemInfo", b => b.Component<SysSystemInfo>().Build());
        Tab.AddTab("SecuritySetting", b => b.Component<SysSystemSafe>().Build());
        Tab.AddTab("WeChatSetting", b => b.Component<WeChatSetting>().Build());
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    internal async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        var result = await systemService.SaveSystemAsync(info);
        if (result.IsValid)
            Context.System = info;
        return result;
    }

    internal Task<Result> SaveKeyAsync(SystemInfo info) => systemService.SaveKeyAsync(info);
}

class SysSystemInfo : BaseForm<SystemInfo>
{
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var data = Parent.Data;
        Model = new FormModel<SystemInfo>(Context)
        {
            Info = new FormInfo { LabelSpan = 4, WrapperSpan = 10 },
            Data = data.System
        };
        Model.AddRow().AddColumn(nameof(SystemInfo.CompName), $"{data.System.CompNo}-{data.System.CompName}");
        Model.AddRow().AddColumn(nameof(SystemInfo.AppName), b =>
        {
            b.Component<KEditInput>()
             .Set(c => c.Value, data.System.AppName)
             .Set(c => c.OnSave, OnSaveAppName)
             .Build();
        });
        Model.AddRow().AddColumn(nameof(VersionInfo.AppVersion), data.Version.AppVersion);
        Model.AddRow().AddColumn(nameof(VersionInfo.SoftVersion), data.Version.SoftVersion);
        Model.AddRow().AddColumn(nameof(VersionInfo.BuildTime), $"{data.Version.BuildTime:yyyy-MM-dd HH:mm:ss}");
        Model.AddRow().AddColumn(nameof(VersionInfo.FrameVersion), data.Version.FrameVersion);
        Model.AddRow().AddColumn(nameof(SystemDataInfo.RunTime), $"{data.RunTime} H");
        if (!Config.App.IsPlatform && !string.IsNullOrWhiteSpace(data.System.ProductId))
        {
            Model.AddRow().AddColumn(nameof(SystemInfo.ProductId), data.System.ProductId);
            Model.AddRow().AddColumn(nameof(SystemInfo.ProductKey), b =>
            {
                b.Component<KEditInput>()
                 .Set(c => c.Value, data.System.ProductKey)
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
        var result = await Parent.SaveSystemAsync(Model.Data);
        if (result.IsValid)
        {
            CurrentUser.AppName = value;
            await App?.StateChangedAsync();
        }
    }

    private async void OnSaveProductKey(string value)
    {
        Model.Data.ProductKey = value;
        await Parent.SaveKeyAsync(Model.Data);
        await StateChangedAsync();
    }
}

class SysSystemSafe : BaseForm<SystemInfo>
{
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var data = Parent.Data;
        Model = new FormModel<SystemInfo>(Context)
        {
            Info = new FormInfo { LabelSpan = 4, WrapperSpan = 10 },
            Data = data.System
        };
        Model.AddRow().AddColumn(nameof(SystemInfo.UserDefaultPwd), b =>
        {
            b.Component<KEditInput>()
             .Set(c => c.Value, data.System.UserDefaultPwd)
             .Set(c => c.OnSave, OnSaveDefaultPwd)
             .Build();
        });
        Model.AddRow().AddColumn(nameof(SystemInfo.IsLoginCaptcha), b =>
        {
            UI.BuildSwitch(b, new InputModel<bool>
            {
                Value = data.System.IsLoginCaptcha,
                ValueChanged = this.Callback<bool>(OnLoginCaptchaChanged)
            });
        });
    }

    protected override void BuildForm(RenderTreeBuilder builder) => builder.FormPage(() => base.BuildForm(builder));

    private async void OnSaveDefaultPwd(string value)
    {
        Model.Data.UserDefaultPwd = value;
        await Parent.SaveSystemAsync(Model.Data);
    }

    private async void OnLoginCaptchaChanged(bool value)
    {
        Model.Data.IsLoginCaptcha = value;
        await Parent.SaveSystemAsync(Model.Data);
    }
}