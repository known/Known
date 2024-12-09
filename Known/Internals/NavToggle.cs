namespace Known.Internals;

class NavToggle : BaseNav
{
    private bool collapsed;

    protected override string Title => collapsed ? Language["Nav.Expand"] : Language["Nav.Collapse"];
    protected override string Icon => collapsed ? "menu-unfold" : "menu-fold";

    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnToggle());

    private void OnToggle()
    {
        collapsed = !collapsed;
        App?.ToggleSide(collapsed);
    }
}