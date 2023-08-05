namespace Sample.Razor;

public sealed class AppRazor
{
    private AppRazor() { }

    public static void Initialize(bool isWeb = true)
    {
        //配置是否Web前后端分离
        KRConfig.IsWeb = isWeb;
        //配置默认首页
        KRConfig.Home = new MenuItem("首页", "fa fa-home", typeof(Home));
    }
}