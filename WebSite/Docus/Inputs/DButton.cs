using WebSite.Docus.Inputs.Buttons;

namespace WebSite.Docus.Inputs;

class DButton : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 支持默认、主要、成功、信息、警告、危险样式
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Button1>("1.文本按钮示例", "block");
        builder.BuildDemo<Button2>("2.图标按钮示例", "block");
        builder.BuildDemo<Button3>("3.禁用按钮示例", "block");
        builder.BuildDemo<Button4>("4.更改名称示例", "block");
    }
}