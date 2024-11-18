namespace Known.Internals;

class SysUserProfileTabs : BaseComponent
{
    private TabModel Tab { get; } = new();

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Tab.AddTab("MyProfile", b => b.Component<UserEditForm>().Build());
        Tab.AddTab("SecuritySetting", b => b.Component<PasswordEditForm>().Build());
        foreach (var item in UIConfig.UserTabs)
        {
            Tab.AddTab(item.Key, item.Value);
        }
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () => builder.Tabs(Tab));
    }
}