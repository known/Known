namespace Known.Internals;

class NavRefresh : BaseNav
{
    protected override string Title => Language["Nav.RefreshPage"];
    protected override string Icon => "reload";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnRefresh());

    private void OnRefresh()
    {
        App?.ReloadPage();
    }
}