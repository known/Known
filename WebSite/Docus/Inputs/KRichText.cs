using WebSite.Docus.Inputs.RichTexts;

namespace WebSite.Docus.Inputs;

class KRichText : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "基于wangEditor.js实现",
            "编辑器配置通过Option参数设置",
            "配置选项参考：https://www.wangeditor.com/v4/"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<RichText1>("1.默认示例", "block");
        builder.BuildDemo<RichText2>("2.事件示例", "block");
        builder.BuildDemo<RichText3>("3.控制示例", "block");
    }
}