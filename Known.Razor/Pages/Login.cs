namespace Known.Razor.Pages;

public class Login : BaseComponent
{
    class LoginInfo
    {
        public string UserName { get; set; }
        public bool Remember { get; set; }
    }

    private readonly string KeyLoginInfo = "LoginInfo";
    private LoginInfo model;
    private Form form;
    private Captcha captcha;
    private string message;

    public Login()
    {
        IsCaptcha = KRConfig.IsWeb;
    }

    [Parameter] public bool IsCaptcha { get; set; }
    [Parameter] public Action<UserInfo> OnLogin { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            model = await UI.GetLocalStorage<LoginInfo>(KeyLoginInfo);
            if (model != null)
            {
                form?.SetData(model);
            }
            UI.InitForm();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("login animated fadeIn", attr =>
        {
            builder.Div("login-left", attr =>
            {
                builder.Div("slogan", Config.AppName);
                builder.Div("image", attr => builder.Img(attr => attr.Src("img/login.jpg")));
            });
            builder.Div("login-form", attr =>
            {
                builder.Div("login-title", "用户登录");
                BuildForm(builder);
            });
        });
    }

    protected void BuildForm(RenderTreeBuilder builder)
    {
        builder.Component<Form>()
               .Set(c => c.ChildContent, BuildFields)
               .Build(value => form = value);
        builder.Div("login-msg", message);
    }

    private void BuildFields(RenderTreeBuilder builder)
    {
        var enter = "$('#btnLogin').click()";
        builder.Field<Text>("UserName", true)
               .Set(f => f.Icon, "fa fa-user-o")
               .Set(f => f.Placeholder, "用户名")
               .Build();
        builder.Field<Password>("Password", true)
               .Set(f => f.Icon, "fa fa-lock")
               .Set(f => f.Placeholder, "密码")
               .Set(f => f.OnEnter, IsCaptcha ? "" : enter)
               .Build();
        if (IsCaptcha)
        {
            builder.Field<Captcha>("Captcha", true)
                   .Set(f => f.Icon, "fa fa-check")
                   .Set(f => f.Placeholder, "验证码")
                   .Set(f => f.OnEnter, enter)
                   .Build(value => captcha = value);
        }
        else
        {
            builder.Field<CheckBox>("Remember")
                   .Set(f => f.Switch, true)
                   .Set(f => f.Text, "记住用户名")
                   .Build();
        }
        builder.Component<Button>()
               .Set(c => c.Id, "btnLogin")
               .Set(c => c.Text, "登 录")
               .Set(c => c.OnClick, Callback(OnUserLogin))
               .Build();
    }

    private void OnUserLogin()
    {
        form?.Submit(async data =>
        {
            if (IsCaptcha)
            {
                string code = data.Captcha;
                if (!captcha.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    message = "验证码不正确！";
                    return;
                }
            }
            await OnUserLogin(data);
        });
    }

    private async Task OnUserLogin(dynamic data)
    {
        var info = new LoginFormInfo
        {
            UserName = data.UserName,
            Password = data.Password
        };
        UI.SetLocalStorage(KeyLoginInfo, new LoginInfo
        {
            UserName = info.UserName,
            Remember = IsCaptcha ? true : Utils.ConvertTo<bool>(data.Remember)
        });
        var result = await Platform.User.SignInAsync(info);
        message = result.Message;
        if (result.IsValid)
        {
            var user = result.DataAs<UserInfo>();
            OnLogin?.Invoke(user);
        }
    }
}