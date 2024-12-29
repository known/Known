namespace Known.Pages;

/// <summary>
/// 首页组件类。
/// </summary>
public class IndexPage : BasePage
{
    /// <summary>
    /// 获取页面标题内容。
    /// </summary>
    /// <returns></returns>
    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("home", "首页");
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        var menu = Context.Current;
        menu ??= new MenuInfo { Id = "Index", Url = "/", Plugins = [] };
        builder.Component<PluginPage>().Set(c => c.Menu, menu).Build();
    }
}