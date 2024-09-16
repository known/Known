namespace Known.AntBlazor.Pages;

/// <summary>
/// 系统开发中心页面组件类。
/// </summary>
[StreamRendering]
[Route("/development")]
public class AntDevelopment : BaseTabPage, IReuseTabsPage
{
    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnPageInitAsync();
        Tab.Class = "kui-development";
        Tab.AddTab("Menu.SysModuleList", b => b.Component<ModuleList>().Build());
        //Tab.AddTab("Designer.CodeGenerator", b => b.Component<CodeGenerateForm>().Build());
        Tab.AddTab("WebApi", BuildWebApi);
    }

    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public RenderFragment GetPageTitle()
    {
        return this.BuildTree(b =>
        {
            b.Icon("appstore-add");
            b.Span(Language["Nav.Development"]);
        });
    }

    private void BuildWebApi(RenderTreeBuilder builder)
    {
        builder.Div().Style("padding:10px;")
               .Children(() => builder.Component<WebApiList>().Build())
               .Close();
    }
}