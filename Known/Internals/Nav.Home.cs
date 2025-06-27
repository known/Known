namespace Known.Internals;

[NavPlugin(Language.Home, "home", Category = Language.Component, Sort = 1)]
class NavHome : BaseNav
{
    protected override string Title => Language.HomePage;
    protected override string Icon => "home";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => Context.GoHomePage());
}