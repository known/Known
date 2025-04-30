namespace Known.Internals;

class NavSetting : BaseNav
{
    [CascadingParameter] private TopNavbar Topbar { get; set; }

    protected override string Title => Language.Setting;
    protected override string Icon => "setting";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => Topbar?.OnSetting?.Invoke());
}