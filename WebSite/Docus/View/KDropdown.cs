using WebSite.Docus.View.Dropdowns;

namespace WebSite.Docus.View;

class KDropdown : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "向下弹出内容"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Dropdown1>("1.默认示例");
    }
}