# 系统登录

## 概述

- 框架内置系统登录页面，也可以对其进行自定义。
- Web程序默认显示图片验证码，单机桌面版默认不显示。


## 自定义页面

- 自定义系统的登录页面只需要重写Login页面和Index页面的BuildLogin方法即可。
- 自定义样式可以在您的项目中重写CSS样式表。

### 1. 重写Login

- 方式一：继承Known.Razor.Pages.Login，重写BuildRenderTree方法，如下示例
- 方式二：完全重新构建Login页面，无需继承Known.Razor.Pages.Login

```csharp
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
                    builder.Div("image", attr => builder.Img(attr => attr.Src("_content/Test.Razor/img/login.jpg")));
                });
                builder.Div("login-form", attr =>
                {
                    builder.Div("login-title", $"{Config.AppId}用户登录");
                    BuildForm(builder);//使用框架默认表单输入
                });
            });
            //自定义版权信息
            builder.Div("copyright", attr =>
            {
                builder.Span($"©2020-{DateTime.Now:yyyy}");
                builder.Anchor("XXX", "http://www.xxx.com");
                builder.Span("版权所有");
            });
        });
    }
}
```

### 2. 重写Index的BuildLogin

```csharp
public class Index : Known.Razor.Pages.Index
{
    protected override void BuildLogin(RenderTreeBuilder builder)
    {
        //Build自定义Login
        builder.Component<Login>().Set(c => c.OnLogin, OnLogin).Build();
    }
}
```

## 效果图

![登录页面](https://foruda.gitee.com/images/1688121567850878119/aeb0ba7c_14334.png "屏幕截图")
![Captcha](https://foruda.gitee.com/images/1688197097755819765/80038310_14334.png "屏幕截图")
