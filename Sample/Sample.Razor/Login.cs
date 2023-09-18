namespace Sample.Razor;

class Login : Known.Razor.Pages.Login
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("login animated fadeIn", attr =>                   //<div class="login animated fadeIn">
        {                                                              //
            builder.Div("login-left", attr =>                          //    <div class="login-left">
            {                                                          //
                builder.Div("slogan", Config.AppName);                 //        <div class="slogan">@Config.AppName</div>
                builder.Div("image", attr =>                           //        <div class="image">
                {                                                      //
                    var url = "_content/Sample.Razor/img/login.jpg";   //
                    builder.Img(attr => attr.Src(url));                //            <img src="_content/Sample.Razor/img/login.jpg" />
                });                                                    //        </div>
            });                                                        //    </div>
            builder.Div("login-form", attr =>                          //    <div class="login-form">
            {                                                          //        
                builder.Div("login-title", $"{Config.AppId}用户登录");  //        <div class="login-title">@(Config.AppId)用户登录</div>
                builder.Div("account", "演示账号：Admin/888888");       //        <div class="account">演示账号：Admin/888888</div>
                BuildForm(builder);                                    //        <LoginForm />
            });                                                        //    </div>
        });                                                            //</div>
        builder.Div("copyright", attr =>                               //<div class="copyright">
        {                                                              //
            builder.Span($"©2020-{DateTime.Now:yyyy}");                //    <span>©2020-@(DateTime.Now.ToString("yyyy"))</span>
            builder.Span("Powered By");                                //    <span>Powered By</span>
            builder.Anchor("Known", "http://known.pumantech.com");     //    <a href="http://known.pumantech.com">Known</a>
        });                                                            //</div>
    }
}