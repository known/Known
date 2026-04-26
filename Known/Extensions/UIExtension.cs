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
    /// <param name="isPDA">是否使用PDA扫码器。</param>
    public static void ShowScanner(this UIService service, Func<string, string, Task> onScan, Func<Task> onStop = null, bool isPDA = false)
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
                              .Set(c => c.IsPDA, isPDA)
                              .Set(c => c.OnScan, async (r, e) =>
                              {
                                  await onScan?.Invoke(r, e);
                                  await model.CloseAsync();
                              })
                              .Set(c => c.OnStop, onStop)
                              .Build(value => scanner = value);
        service.ShowDialog(model);
    }

    /// <summary>
    /// 显示AI聊天抽屉。
    /// </summary>
    /// <param name="service">UI服务实例。</param>
    /// <param name="info">AI代理信息。</param>
    public static void ShowAIDrawer(this UIService service, AgentInfo info)
    {
        var model = new DrawerModel
        {
            ClassName = "kai-drawer",
            Title = info.Name,
            Width = "700px",
            MaskClosable = false,
            Content = b => b.Component<ChatView>().Set(c => c.Agent, info).Build()
        };
        service.ShowDrawer(model);
    }

    /// <summary>
    /// 显示AI聊天抽屉。
    /// </summary>
    /// <typeparam name="TComponent">抽屉组件类型。</typeparam>
    /// <param name="service">UI服务实例。</param>
    /// <param name="title">抽屉标题。</param>
    /// <param name="width">抽屉宽度。</param>
    /// <param name="parameters">组件参数委托。</param>
    public static void ShowAIDrawer<TComponent>(this UIService service, string title, string width = "700px", Action<ComponentBuilder<TComponent>> parameters = null)
        where TComponent : Microsoft.AspNetCore.Components.IComponent
    {
        var model = new DrawerModel
        {
            ClassName = "kai-drawer",
            Title = title,
            Width = width,
            MaskClosable = false,
            Content = b =>
            {
                var component = b.Component<TComponent>();
                parameters?.Invoke(component);
                component.Build();
            }
        };
        service.ShowDrawer(model);
    }
}