# 系统设置

本章介绍系统主题设置功能。

## 概述

- 框架内置系统后台主页，也可以对其布局进行自定义。
- 框架主题布局支持两种，顶部+侧边栏背景色和仅侧边栏有背景色。
- 框架页面支持单页和多标签页设置，默认为单页。
- 框架主题颜色支持自定义设置。

## 自定义

自定义后台管理主页面只需要重写Admin页面和Index页面的BuildAdmin方法即可。

### 1. 重写Admin

- 方式一：使用框架内置Admin模板
- 方式二：完全重新构建Admin页面

```csharp
class CustomAdmin : BaseComponent
{
    private bool isInitialized;
    private AdminInfo? info;

    [Parameter] public Action? OnLogout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        info = await Platform.User.GetAdminAsync();
        KRConfig.UserMenus = info.UserMenus;   //用户菜单
        Setting.UserSetting = info.UserSetting;//用户个性化设置
        if (Context.IsWebApi)
            Cache.AttachCodes(info.Codes);     //缓存数据字典
        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        builder.Div("app", attr =>
        {
            builder.Component<CustomHeader>() //自定义Header
                   .Set(c => c.Info, info)
                   .Set(c => c.OnLogout, OnLogout)
                   .Build();
            builder.Component<CustomSider>().Build(); //自定义Sider
            builder.Component<AdminBody>().Build();//直接使用框架内置Body
        });
        builder.Component<DialogContainer>().Build(); //Dialog容器
    }
}
```

### 2. 重写Index的BuildAdmin

```csharp
public class Index : Known.Razor.Pages.Index
{
    protected override void BuildAdmin(RenderTreeBuilder builder)
    {
        //Build自定义Admin
        builder.Component<CustomAdmin>().Set(c => c.OnLogout, OnLogout).Build();
    }
}
```

## 效果图

![桌面主页](https://foruda.gitee.com/images/1688092817417883098/53a1da51_14334.png "屏幕截图")
