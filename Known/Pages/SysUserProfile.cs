namespace Known.Pages;

/// <summary>
/// 用户个人中心页面组件类。
/// </summary>
public class SysUserProfile : BasePage<SysUser>
{
    internal IUserService Service { get; private set; }
    internal SysUser User { get; private set; }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IUserService>();
        User = await Service.GetUserAsync(CurrentUser.Id);

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card", BuildUserInfo);
        Page.AddItem("kui-card", BuildUserTabs);
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Div("kui-p10", () => builder.Component<SysUserProfileInfo>().Build());
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();
}

class SysUserProfileInfo : BaseComponent
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var User = Parent.User;
        builder.Div("kui-user-avatar", () =>
        {
            builder.Markup($"<img src=\"{CurrentUser?.AvatarUrl}\" />");
            builder.Div("kui-upload-button", () =>
            {
                builder.Icon("edit");
                builder.Span(Language.Edit);
                builder.Component<InputFile>()
                       .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnFileChanged))
                       .Build();
            });
        });
        builder.Ul("kui-user-info", () =>
        {
            BuildUserInfoItem(builder, "user", $"{User?.Name}({User?.UserName})");
            BuildUserInfoItem(builder, "phone", User?.Phone);
            BuildUserInfoItem(builder, "mobile", User?.Mobile);
            BuildUserInfoItem(builder, "inbox", User?.Email);
            BuildUserInfoItem(builder, "team", User?.Role);
            BuildUserInfoItem(builder, "comment", User?.Note);
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

    private async void OnFileChanged(InputFileChangeEventArgs e)
    {
        var file = await e.File.CreateFileAsync();
        var info = new AvatarInfo { UserId = Parent?.User?.Id, File = file };
        var result = await Parent?.Service?.UpdateAvatarAsync(info);
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