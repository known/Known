namespace Known.Studio.Pages;

[Route("/")]
public class Index : BaseComponent
{
    private bool isLoaded;
    private static readonly List<MenuItem> menus =
    [
        new MenuItem("代码生成", "fa fa-code", typeof(DevCode))
    ];
    private MenuItem curItem = menus[0];

    protected override void OnInitialized()
    {
        //Context.CurrentUser = new UserInfo { UserName = Constants.SysUserName };
        isLoaded = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isLoaded)
            return;

        //builder.Div("app", attr =>
        //{
        //    builder.Div("header", "Known开发工具");
        //    builder.Div("sider", attr =>
        //    {
        //        builder.Div("fa fa-university logo");
        //        builder.Component<KMenu>()
        //               .Set(c => c.Style, "menu menu1")
        //               .Set(c => c.OnlyIcon, true)
        //               .Set(c => c.Items, menus)
        //               .Set(c => c.OnClick, item => curItem = item)
        //               .Build();
        //    });
        //    builder.Div("title", curItem.Name);
        //    builder.Div("content", attr => builder.DynamicComponent(curItem.ComType));
        //});
    }
}