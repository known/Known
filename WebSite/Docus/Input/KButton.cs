using WebSite.Docus.Input.Buttons;

namespace WebSite.Docus.Input;

class KButton : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持默认、主要、成功、信息、警告、危险样式"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Button1>();
        builder.BuildDemo<Button2>();
        builder.BuildDemo<Button3>();
    }
}