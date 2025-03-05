namespace Known.Internals;

/// <summary>
/// 附件连接组件类型。
/// </summary>
public class FileLink : BaseComponent
{
    /// <summary>
    /// 取得或设置附件信息。
    /// </summary>
    [Parameter] public AttachInfo Item { get; set; }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Config.App.Type == AppType.Web)
            builder.OpenFile(Item.Name, Item.FileUrl);
        else
            builder.Span("kui-link", Item.Name, this.Callback<MouseEventArgs>(e => OnDownloadFileAsync(Item)));
    }

    private Task OnDownloadFileAsync(AttachInfo item)
    {
        return App?.DownloadAsync(async () =>
        {
            var path = Config.GetUploadPath(item.Path);
            if (!File.Exists(path))
                return;

            var bytes = await File.ReadAllBytesAsync(path);
            await JS.DownloadFileAsync(item.SourceName, bytes);
        });
    }
}