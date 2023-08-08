namespace Sample.Razor;

class Login : Known.Razor.Pages.Login
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("login animated fadeIn", attr =>
        {
            builder.Div("login-left", attr =>
            {
                builder.Div("slogan", Config.AppName);
                builder.Div("image", attr => builder.Img(attr => attr.Src("_content/Sample.Razor/img/login.jpg")));
            });
            builder.Div("login-form", attr =>
            {
                builder.Div("login-title", $"{Config.AppId}用户登录");
                builder.Div("account", "演示账号：Admin/888888");
                BuildForm(builder);
            });
        });
        builder.Div("copyright", attr =>
        {
            builder.Span($"©2020-{DateTime.Now:yyyy}");
            builder.Span("Powered By");
            builder.Anchor("Known", "http://known.pumantech.com");
        });
    }
}