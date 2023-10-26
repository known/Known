namespace WebSite.Docus.Basic;

class DIcon : BaseDocu
{
    class FaIcon
    {
        public string? Name { get; set; }
        public string? Unicode { get; set; }

        public string FA => $"fa fa-{Name}";
    }

    private bool isLoaded;
    private List<FaIcon>? icons;
    [Inject] private HttpClient Http { get; set; }

    protected override async Task OnInitializedAsync()
    {
        isLoaded = false;
        icons = await Http.GetFromJsonAsync<List<FaIcon>>("https://fontawesome.com.cn/api/fav4");
        isLoaded = true;
    }

    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 默认内置Font Awesome字体库
- 参考网站：[FontAwesome中文网](https://fontawesome.com.cn/)
- 点击下方示例可复制到剪切板
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        if (!isLoaded) return;
        if (icons == null || icons.Count == 0) return;

        builder.Ul("icons", attr =>
        {
            foreach (var item in icons)
            {
                builder.Li(item.FA, attr =>
                {
                    attr.OnClick(Callback(e=>OnIconClick(item)));
                    builder.Span(item.Name);
                    builder.Span(item.Unicode);
                });
            }
        });
    }

    private void OnIconClick(FaIcon item)
    {
        UI.CopyToClipboard(item.FA);
    }
}