namespace Known.Extensions;

/// <summary>
/// 模态对话框扩展类。
/// </summary>
public static class ModalExtension
{
    /// <summary>
    /// 预览附件。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="files">附件列表。</param>
    public static void PreviewFile(this UIService service, List<AttachInfo> files)
    {
        var model = new DialogModel
        {
            Title = Language.PreviewFile,
            Width = 800,
            Maximizable = true,
            Content = b => b.Component<KFileView>().Set(c => c.Items, files).Build()
        };
        service.ShowDialog(model);
    }
}