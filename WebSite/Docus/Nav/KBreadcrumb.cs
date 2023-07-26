using WebSite.Docus.Nav.Breadcrumbs;

namespace WebSite.Docus.Nav;

class KBreadcrumb : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "数据源支持当前菜单和MenuItem列表",
            "可定义图标和MenuItem事件"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Breadcrumb1>("1.默认示例");
        builder.BuildDemo<Breadcrumb2>("2.事件示例");
    }
}