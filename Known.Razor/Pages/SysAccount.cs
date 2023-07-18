using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysAccount : PageComponent
{
    private readonly List<MenuItem> items = new();
    private SysUserInfo userInfo;

    protected override void OnInitialized()
    {
        if (KRConfig.IsWeb)
            items.Add(new MenuItem("我的消息", "fa fa-envelope-o", typeof(SysMyMessage)));
        items.Add(new MenuItem("我的信息", "fa fa-user", typeof(SysAccountForm)));
        items.Add(new MenuItem("安全设置", "fa fa-lock", typeof(SysUserPwdForm)));
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Cascading(this, b =>
        {
            b.Div("left-view", attr => BuildUserInfo(b));
            b.Div("right-view", attr => BuildUserTabs(b));
        });
    }

    internal void RefreshUserInfo() => userInfo.Refresh();

    private void BuildUserInfo(RenderTreeBuilder builder)
    {
        builder.Component<SysUserInfo>().Build(value => userInfo = value);
    }

    private void BuildUserTabs(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}