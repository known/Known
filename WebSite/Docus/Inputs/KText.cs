using WebSite.Docus.Inputs.Texts;

namespace WebSite.Docus.Inputs;

class KText : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "文本字符输入框"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Text1>("1.默认示例");
        builder.BuildDemo<Text2>("2.事件示例");
        builder.BuildDemo<Text3>("3.控制示例");
    }
}