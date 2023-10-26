using WebSite.Docus.Feedback.Toasts;

namespace WebSite.Docus.Feedback;

class DToast : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 提示位置默认顶上居中
- 支持默认、主要、成功、信息、警告、危险样式
- 默认3000毫秒后自动关闭
- 内容支持html字符
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Toast1>("1.默认示例");
        builder.BuildDemo<Toast2>("2.自定义示例");
    }
}