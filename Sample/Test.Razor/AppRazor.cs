namespace Test.Razor;

public class AppRazor
{
    public static void Initialize()
    {
        KRConfig.Home = new MenuItem("首页", "fa fa-home", typeof(Home));
    }
}