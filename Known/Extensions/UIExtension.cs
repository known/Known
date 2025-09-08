namespace Known.Extensions;

/// <summary>
/// UI服务扩展类。
/// </summary>
public static class UIExtension
{
    /// <summary>
    /// 显示浏览器摄像头扫码窗口。
    /// </summary>
    /// <param name="service">UI服务实例。</param>
    /// <param name="onScan">扫码结果回调。</param>
    /// <param name="onStop">停止扫码回调。</param>
    public static void ShowScanner(this UIService service, Func<string, string, Task> onScan, Func<Task> onStop = null)
    {
        Scanner scanner = null;
        var model = new DialogModel
        {
            ClassName = "kui-scanner",
            Style = "width:280px;height:200px;",
            OnClosed = () => scanner?.StopAsync()
        };
        model.Content = b => b.Component<Scanner>()
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