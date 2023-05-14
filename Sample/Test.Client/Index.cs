namespace Test.Client;

[Route("/")]
public class Index : Known.Razor.Pages.Index
{
    protected override void BuildLogin(RenderTreeBuilder builder)
    {
        builder.Component<Login>().Set(c => c.OnLogin, OnLogin).Build();
    }
}

class Login : Known.Razor.Pages.Login
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("login", attr =>
        {
            builder.Div("login-box", attr =>
            {
                builder.Div("login-left", attr =>
                {
                    builder.Div("slogan", Config.AppName);
                    builder.Div("image", attr => builder.Img(attr => attr.Src("img/login.jpg")));
                });
                builder.Div("login-form", attr =>
                {
                    builder.Div("login-title", $"{Config.AppId}用户登录");
                    BuildForm(builder);
                });
            });
            builder.Div("copyright", attr =>
            {
                builder.Span($"©2020-{DateTime.Now:yyyy}");
                builder.Anchor("而微云", "http://www.ewyun.com.cn");
                builder.Span("版权所有");
            });
        });
    }
}