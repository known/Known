namespace Known.Extensions;

/// <summary>
/// UI服务扩展类。
/// </summary>
public static class UIExtension
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

    /// <summary>
    /// 显示浏览器摄像头扫码窗口。
    /// </summary>
    /// <param name="service">UI服务实例。</param>
    /// <param name="onScan">扫码结果回调。</param>
    /// <param name="onStop">停止扫码回调。</param>
    public static void ShowScanner(this UIService service, Func<string, string, Task> onScan, Func<Task> onStop = null)
    {
        KScanner scanner = null;
        var model = new DialogModel
        {
            ClassName = "kui-scanner-modal",
            Style = "width:260px;height:220px;",
            OnClosed = () => scanner?.StopAsync()
        };
        model.Content = b => b.Component<KScanner>()
                              .Set(c => c.AutoStart, true)
                              .Set(c => c.OnScan, async (r, e) =>
                              {
                                  await onScan?.Invoke(r, e);
                                  await model.CloseAsync();
                              })
                              .Set(c => c.OnStop, onStop)
                              .Build(value => scanner = value);
        service.ShowDialog(model);
    }
}