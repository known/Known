using WebSite.Docus.Input.Captchas;

namespace WebSite.Docus.Input;

class KCaptcha : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "基于Canvas实现",
            "字母不区分大小写",
            "点击图片可刷新验证码"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Captcha1>();
        builder.BuildDemo<Captcha2>();
    }
}