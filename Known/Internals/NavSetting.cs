namespace Known.Internals;

[NavItem]
class NavSetting : BaseNav
{
    [CascadingParameter] private TopNavbar Topbar { get; set; }

    protected override string Title => Language["Nav.Setting"];
    protected override string Icon => "setting";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => Topbar?.OnMenuClick?.Invoke("setting"));
}