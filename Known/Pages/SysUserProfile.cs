namespace Known.Pages;

[Route("/profile")]
public class SysUserProfile : BasePage<SysUser>
{
    private SysUserProfileInfo info;
    internal SysUser User { get; private set; }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();

        User = await Platform.User.GetUserAsync(CurrentUser.Id);

        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card", BuildUserInfo);
        Page.AddItem("kui-card", BuildUserTabs);
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Div("p10", () => builder.Component<SysUserProfileInfo>().Build(value => info = value));
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();

    internal void UpdateProfileInfo() => info?.StateChanged();
}

class SysUserProfileInfo : BaseComponent
{
    [CascadingParameter] private SysUserProfile Parent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var User = Parent.User;
        builder.Div("kui-user-avatar", () => builder.Markup($"<img src=\"{CurrentUser?.AvatarUrl}\" />"));
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

    private void BuildUserInfoItem(RenderTreeBuilder builder, string icon, string text)
    {
        builder.Li(() =>
        {
            UI.Icon(builder, icon);
            builder.Span(text);
        });
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