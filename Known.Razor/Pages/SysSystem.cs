using Known.Razor.Pages.Forms;

namespace Known.Razor.Pages;

class SysSystem : PageComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("系统信息", "fa fa-info-circle", typeof(SysSystemInfo)),
        new MenuItem("安全设置", "fa fa-shield", typeof(SysSecurityInfo))
    };

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}