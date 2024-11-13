namespace Known.Pages;

/// <summary>
/// 用户个人中心页面组件类。
/// </summary>
public class SysUserProfile : BasePage<UserInfo>
{
    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card kui-p10", BuildUserInfo);
        Page.AddItem("kui-card", BuildUserTabs);
    }

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Component<SysUserProfileInfo>().Build();
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();
}

class SysUserProfileInfo : BaseComponent
{
    private IAuthService Service;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IAuthService>();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var user = CurrentUser;
        builder.Div("kui-user-avatar", () =>
        {
            builder.Markup($"<img src=\"{user?.AvatarUrl}\" />");
            builder.Div("kui-upload-button", () =>
            {
                builder.Icon("edit");
                builder.Span(Language.Edit);
                builder.Component<InputFile>()
                       .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnFileChangedAsync))
                       .Build();
            });
        });
        builder.Ul("kui-user-info", () =>
        {
            BuildUserInfoItem(builder, "user", $"{user?.Name}({user?.UserName})");
            BuildUserInfoItem(builder, "phone", user?.Phone);
            BuildUserInfoItem(builder, "mobile", user?.Mobile);
            BuildUserInfoItem(builder, "inbox", user?.Email);
            BuildUserInfoItem(builder, "team", user?.Role);
            BuildUserInfoItem(builder, "comment", user?.Note);
        });
    }

    private static void BuildUserInfoItem(RenderTreeBuilder builder, string icon, string text)
    {
        builder.Li(() =>
        {
            builder.Icon(icon);
            builder.Span(text);
        });
    }

    private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
    {
        var file = await e.File.CreateFileAsync();
        var info = new AvatarInfo { UserId = CurrentUser?.Id, File = file };
        var result = await Service?.UpdateAvatarAsync(info);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
            return;
        }

        var user = CurrentUser;
        user.AvatarUrl = result.DataAs<string>();
        App?.SetCurrentUserAsync(user);
        Navigation.Refresh(true);
    }
}

class SysUserProfileTabs : BaseComponent
{
    private TabModel Tab { get; } = new();

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        //Tab.AddTab("TodoList", b => b.Component<SysSystemInfo>().Build());
        //Tab.AddTab("MyMessage", b => b.Component<SysSystemSafe>().Build());
        Tab.AddTab("MyProfile", b => b.Component<UserEditForm>().Build());
        Tab.AddTab("SecuritySetting", b => b.Component<PasswordEditForm>().Build());
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => UI.BuildTabs(builder, Tab));
    }
}