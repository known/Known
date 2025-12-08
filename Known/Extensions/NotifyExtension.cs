namespace Known.Extensions;

/// <summary>
/// 通知服务扩展类。
/// </summary>
public static class NotifyExtension
{
    /// <summary>
    /// 发送模板通知消息。
    /// </summary>
    /// <param name="service">通知服务实例。</param>
    /// <param name="title">通知标题。</param>
    /// <param name="message">通知内容。</param>
    /// <param name="type">通知窗口类型。</param>
    /// <returns></returns>
    public static Task LayoutNotifyAsync(this INotifyService service, string title, string message, StyleType type = StyleType.Success)
    {
        return service.NotifyAsync(Constants.NotifyLayout, title, message, type);
    }

    /// <summary>
    /// 发送模板通知消息。
    /// </summary>
    /// <param name="service">通知服务实例。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="title">通知标题。</param>
    /// <param name="message">通知内容。</param>
    /// <param name="type">通知窗口类型。</param>
    /// <returns></returns>
    public static Task LayoutNotifyAsync(this INotifyService service, string userName, string title, string message, StyleType type = StyleType.Success)
    {
        return service.NotifyAsync(userName, Constants.NotifyLayout, title, message, type);
    }

    /// <summary>
    /// 发送模板通知消息。
    /// </summary>
    /// <param name="service">通知服务实例。</param>
    /// <param name="method">通知方法名。</param>
    /// <param name="title">通知标题。</param>
    /// <param name="message">通知内容。</param>
    /// <param name="type">通知窗口类型。</param>
    /// <returns></returns>
    public static Task NotifyAsync(this INotifyService service, string method, string title, string message, StyleType type = StyleType.Success)
    {
        var info = new NotifyInfo { Type = type, Title = title, Message = message };
        return service.SendAsync(method, info);
    }

    /// <summary>
    /// 发送模板通知消息。
    /// </summary>
    /// <param name="service">通知服务实例。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="method">通知方法名。</param>
    /// <param name="title">通知标题。</param>
    /// <param name="message">通知内容。</param>
    /// <param name="type">通知窗口类型。</param>
    /// <returns></returns>
    public static Task NotifyAsync(this INotifyService service, string userName, string method, string title, string message, StyleType type = StyleType.Success)
    {
        var info = new NotifyInfo { Type = type, Title = title, Message = message };
        return service.SendAsync(userName, method, info);
    }

    internal static Task NotifyOnlineAsync(this INotifyService service)
    {
        return service.SendAsync(Constants.KeyOnline, "");
    }
}