namespace WebSite.Docus.Inputs.SearchBoxs;

class SearchBox1 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KSearchBox>()
               .Set(c => c.Required, true)
               .Set(c => c.Placeholder, "请输入关键字")
               .Set(c => c.OnSearh, OnSearh)
               .Build();
        builder.Div("tips", message);
    }

    private void OnSearh(string key)
    {
        message = key;
        StateChanged();
    }
}