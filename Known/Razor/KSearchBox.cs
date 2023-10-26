using Known.Extensions;

namespace Known.Razor;

public class KSearchBox : BaseComponent
{
    private string error;
    private string key;

    [Parameter] public bool Required { get; set; } = true;
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public Action<string> OnSearh { get; set; }

    public bool Validate()
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(key) && Required)
        {
            error = "error";
            return false;
        }

        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("search-box").AddClass(error).Build();
        builder.Div(css, attr =>
        {
            builder.Input(attr => attr.Placeholder(Placeholder).OnChange(Callback<ChangeEventArgs>(e => OnKeyChanged(e))));
            builder.Icon("fa fa-search", "", Callback(OnClick));
        });
    }

    private void OnKeyChanged(ChangeEventArgs e) => key = e?.Value?.ToString();

    private void OnClick()
    {
        if (!Validate())
            return;

        OnSearh?.Invoke(key);
    }
}