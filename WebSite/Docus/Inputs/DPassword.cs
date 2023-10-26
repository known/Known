using WebSite.Docus.Inputs.Passwords;

namespace WebSite.Docus.Inputs;

class DPassword : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 密码字段组件
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Password1>("1.默认示例");
    }
}