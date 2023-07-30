using WebSite.Docus.Inputs.RichTexts;

namespace WebSite.Docus.Inputs;

class KRichText : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "基于wangEditor.js实现"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<RichText1>("1.默认示例", "block");
        builder.BuildDemo<RichText2>("2.事件示例", "block");
        builder.BuildDemo<RichText3>("3.控制示例", "block");
    }
}