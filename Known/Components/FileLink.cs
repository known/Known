namespace Known.Components;

class FileLink : BaseComponent
{
    [Parameter] public SysFile Item { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Config.App.Type == AppType.Web)
            builder.OpenFile(Item.Name, Item.FileUrl);
        else
            builder.Span("kui-link", Item.Name, this.Callback<MouseEventArgs>(e => OnDownloadFile(Item)));
    }

    private async void OnDownloadFile(SysFile item)
    {
        var path = Config.GetUploadPath(item.Path);
        if (!File.Exists(path))
            return;

        var bytes = await File.ReadAllBytesAsync(path);
        if (bytes != null && bytes.Length > 0)
        {
            var stream = new MemoryStream(bytes);
            await JS.DownloadFileAsync(item.SourceName, stream);
        }
    }
}