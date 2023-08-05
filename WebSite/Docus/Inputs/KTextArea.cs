using WebSite.Docus.Inputs.TextAreas;

namespace WebSite.Docus.Inputs;

class KTextArea : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 多行文本字符输入框
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<TextArea1>("1.默认示例");
        builder.BuildDemo<TextArea2>("2.事件示例");
        builder.BuildDemo<TextArea3>("3.控制示例");
    }
}