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
    public static async void Success(this UIService service, string message)
    {
        await service.SuccessAsync(message);
    }

    /// <summary>
    /// 异步呈现成功提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    /// <returns></returns>
    public static Task SuccessAsync(this UIService service, string message)
    {
        return service.Toast(message, StyleType.Success);
    }

    /// <summary>
    /// 呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    public static async void Info(this UIService service, string message)
    {
        await service.InfoAsync(message);
    }

    /// <summary>
    /// 异步呈现信息提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">提示文本。</param>
    /// <returns></returns>
    public static Task InfoAsync(this UIService service, string message)
    {
        return service.Toast(message, StyleType.Info);
    }

    /// <summary>
    /// 呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    public static async void Warning(this UIService service, string message)
    {
        await service.WarningAsync(message);
    }

    /// <summary>
    /// 异步呈现警告提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">警告文本。</param>
    /// <returns></returns>
    public static Task WarningAsync(this UIService service, string message)
    {
        return service.Toast(message, StyleType.Warning);
    }

    /// <summary>
    /// 呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    public static async void Error(this UIService service, string message)
    {
        await service.ErrorAsync(message);
    }

    /// <summary>
    /// 异步呈现错误提示。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="message">错误文本。</param>
    /// <returns></returns>
    public static Task ErrorAsync(this UIService service, string message)
    {
        return service.Toast(message, StyleType.Error);
    }

    /// <summary>
    /// 显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    public static async void Result(this UIService service, Result result, Func<Task> onSuccess = null)
    {
        await service.ResultAsync(result, onSuccess);
    }

    /// <summary>
    /// 异步显示后端返回的操作结果。
    /// </summary>
    /// <param name="service">UI服务。</param>
    /// <param name="result">后端操作结果。</param>
    /// <param name="onSuccess">后端操作成功回调委托。</param>
    /// <returns></returns>
    public static async Task ResultAsync(this UIService service, Result result, Func<Task> onSuccess = null)
    {
        if (!result.IsValid)
        {
            await service.ErrorAsync(result.Message);
            return;
        }

        if (onSuccess != null)
            await onSuccess.Invoke();
        await service.Toast(result.Message);
    }
}