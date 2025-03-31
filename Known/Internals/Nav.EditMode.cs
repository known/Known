namespace Known.Internals;

class NavEditMode : BaseNav
{
    protected override string Title => Language["Nav.EditMode"];
    protected override string Icon => "highlight";
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnEdit());

    private void OnEdit()
    {
        Context.IsEditMode = !Context.IsEditMode;
        App.StateChanged();
        App.ReloadPage();
    }
}