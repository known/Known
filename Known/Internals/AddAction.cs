namespace Known.Internals;

class AddAction : BaseComponent
{
    [Parameter] public string Type { get; set; }
    [Parameter] public Action OnRefresh { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Class("kui-edit").OnClick(this.Callback<MouseEventArgs>(OnClick)).Child(() =>
        {
            builder.Component<KIcon>()
                   .Set(c => c.Icon, "plus")
                   .Build();
        });
    }

    private void OnClick(MouseEventArgs e)
    {
        var model = new DialogModel();
        model.Title = "在线设计";
        UI.ShowDialog(model);
    }
}