namespace Known.Internals;

[NavPlugin("首页", "home", Category = "组件", Sort = 1)]
class NavHome : BaseNav
{
    protected override string Title => Language["Nav.HomePage"];
    protected override string Icon => "home";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => Context.GoHomePage());
}