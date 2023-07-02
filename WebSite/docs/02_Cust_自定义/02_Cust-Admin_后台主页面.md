# 自定义后台主页面
自定义后台管理主页面只需要重写Admin页面和Index页面的BuildAdmin方法即可。
## 1. 重写Admin
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

## 2. 重写Index的BuildAdmin
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