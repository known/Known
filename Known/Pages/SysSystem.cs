namespace Known.Pages;

class SysSystem : PageComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem("系统信息", "fa fa-info-circle", typeof(SysSystemInfo)),
        new KMenuItem("安全设置", "fa fa-shield", typeof(SysSecurityInfo))
    };

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}