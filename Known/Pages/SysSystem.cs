namespace Known.Pages;

[Route("/sys/info")]
public class SysSystem : BaseTabPage
{
    private SysSystemInfo info;
    private SysSystemSafe safe;
    private WeChatSetting weChat;
    internal SystemInfo Data { get; private set; }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Data = await Platform.System.GetSystemAsync();

        Tab.AddTab("SystemInfo", b => b.Component<SysSystemInfo>().Build(value => info = value));
        Tab.AddTab("SecuritySetting", b => b.Component<SysSystemSafe>().Build(value => safe = value));
        Tab.AddTab("WeChatSetting", b => b.Component<WeChatSetting>().Build(value => weChat = value));
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    public override void StateChanged()
    {
        info?.StateChanged();
        safe?.StateChanged();
        weChat?.StateChanged();
        base.StateChanged();
    }
}

class SysSystemInfo : BaseForm<SystemInfo>
{
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
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

        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder) => builder.FormPage(() => base.BuildForm(builder));

    private async void OnSaveAppName(string value)
    {
        Model.Data.AppName = value;
        var result = await Platform.System.SaveSystemAsync(Model.Data);
        if (result.IsValid)
        {
            CurrentUser.AppName = value;
            App?.StateChanged();
        }
    }

    private async void OnSaveProductKey(string value)
    {
        Model.Data.ProductKey = value;
        await Platform.System.SaveKeyAsync(Model.Data);
        StateChanged();
    }
}

class SysSystemSafe : BaseForm<SystemInfo>
{
    [CascadingParameter] private SysSystem Parent { get; set; }

    protected override async Task OnInitFormAsync()
    {
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

        await base.OnInitFormAsync();
    }

    protected override void BuildForm(RenderTreeBuilder builder) => builder.FormPage(() => base.BuildForm(builder));

    private async void OnSaveDefaultPwd(string value)
    {
        Model.Data.UserDefaultPwd = value;
        await Platform.System.SaveSystemAsync(Model.Data);
    }

    private async void OnLoginCaptchaChanged(bool value)
    {
        Model.Data.IsLoginCaptcha = value;
        await Platform.System.SaveSystemAsync(Model.Data);
    }
}