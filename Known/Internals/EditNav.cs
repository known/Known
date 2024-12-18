namespace Known.Internals;

class EditNav : BaseComponent
{
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Class("kui-edit").OnClick(this.Callback<MouseEventArgs>(OnClick)).Child(() =>
        {
            builder.Component<KIcon>().Set(c => c.Title, "添加导航").Set(c => c.Icon, "plus").Build();
        });
    }

    private void OnClick(MouseEventArgs e)
    {
        var model = new DialogModel();
        model.Title = "添加导航";
        UI.ShowDialog(model);
    }
}