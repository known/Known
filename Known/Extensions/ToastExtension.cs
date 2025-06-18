namespace Known.Extensions;

/// <summary>
/// 弹层提示组件扩展类。
/// </summary>
public static class ToastExtension
{
    /// <summary>
    /// 呈现成功提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    public static void Success(this UIService service, string message)
    {
        service.SuccessAsync(message);
    }

    /// <summary>
    /// 异步呈现成功提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    /// <returns></returns>
    public static Task SuccessAsync(this UIService service, string message)
    {
        return service.ToastAsync(message, StyleType.Success);
    }

    /// <summary>
    /// 呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    public static void Info(this UIService service, string message)
    {
        service.InfoAsync(message);
    }

    /// <summary>
    /// 异步呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    /// <returns></returns>
    public static Task InfoAsync(this UIService service, string message)
    {
        return service.ToastAsync(message, StyleType.Info);
    }

    /// <summary>
    /// 呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    public static void Warning(this UIService service, string message)
    {
        service.WarningAsync(message);
    }

    /// <summary>
    /// 异步呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    /// <returns></returns>
    public static Task WarningAsync(this UIService service, string message)
    {
        return service.ToastAsync(message, StyleType.Warning);
    }

    /// <summary>
    /// 呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    public static void Error(this UIService service, string message)
    {
        service.ErrorAsync(message);
    }

    /// <summary>
    /// 异步呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    /// <returns></returns>
    public static Task ErrorAsync(this UIService service, string message)
    {
        return service.ToastAsync(message, StyleType.Error);
    }

    /// <summary>
    /// 显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    public static void Result(this UIService service, Result result, Func<Task> onSuccess = null)
    {
        service.ResultAsync(result, onSuccess);
    }

    /// <summary>
    /// 异步显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    /// <returns></returns>
    public static Task ResultAsync(this UIService service, Result result, Func<Task> onSuccess = null)
    {
        if (!result.IsValid)
        {
            service.ErrorAsync(result.Message);
            return Task.CompletedTask;
        }

        onSuccess?.Invoke();
        return service.ToastAsync(result.Message);
    }
}