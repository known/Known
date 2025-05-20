namespace Known.Components;

/// <summary>
/// 附件连接组件类型。
/// </summary>
public class KFileLink : BaseComponent
{
    /// <summary>
    /// 取得或设置附件信息。
    /// </summary>
    [Parameter] public FileUrlInfo Url { get; set; }

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
        if (Config.App.Type != AppType.Web)
        {
            builder.Component<KLink>()
                   .Set(c => c.Name, Item.Name)
                   .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e => OnDownloadFileAsync(Item)))
                   .Build();
            return;
        }

        var name = Item != null ? Item.Name : Name;
        var url = Item != null ? Item.FileUrl : Url;
        builder.OpenFile(name, url);
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