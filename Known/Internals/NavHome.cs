namespace Known.Internals;

[NavItem]
class NavHome : BaseNav
{
    protected override string Title => Language["Nav.HomePage"];
    protected override string Icon => "home";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => Navigation.NavigateTo("/"));
}