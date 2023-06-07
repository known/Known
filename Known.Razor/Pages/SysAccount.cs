using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysAccount : PageComponent
{
    private readonly List<MenuItem> items = new();
    private MenuItem curItem;

    protected override void OnInitialized()
    {
        if (KRConfig.IsWeb)
            items.Add(new MenuItem("我的消息", "fa fa-envelope-o", typeof(SysMyMessage)));
        items.Add(new MenuItem("我的信息", "fa fa-user", typeof(SysAccountForm)));
        items.Add(new MenuItem("安全设置", "fa fa-lock", typeof(SysUserPwdForm)));
        items.Add(new MenuItem("系统设置", "fa fa-cog", typeof(SysSettingForm)));
        curItem = items[0];
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var user = CurrentUser;
        builder.Div("ss-form", attr =>
        {
            builder.Div("leftBar box", attr =>
            {
                builder.Img(attr => attr.Src($"_content/Known.Razor{user?.AvatarUrl}"));
                builder.Div("name", user?.Name);
                builder.Component<Tab>()
                   .Set(c => c.Position, "left")
                   .Set(c => c.CurItem, curItem.Id)
                   .Set(c => c.Items, items)
                   .Set(c => c.OnChanged, OnTabChanged)
                   .Build();
            });
            builder.DynamicComponent(curItem.ComType);
        });
    }

    private void OnTabChanged(MenuItem item)
    {
        curItem = item;
        StateChanged();
    }
}