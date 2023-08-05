using WebSite.Docus.Inputs.Captchas;

namespace WebSite.Docus.Inputs;

class KCaptcha : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于Canvas实现
- 字母不区分大小写
- 点击图片可刷新验证码
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Captcha1>("1.默认示例");
        builder.BuildDemo<Captcha2>("2.验证示例");
    }
}