using WebSite.Docus.Nav.Menus;

namespace WebSite.Docus.Nav;

class DMenu : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 支持折叠和选项卡模式
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Menu1>("1.默认菜单示例");
        builder.BuildDemo<Menu2>("2.选项卡菜单示例");
    }
}