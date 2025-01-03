namespace Known;

/// <summary>
/// 客户端配置选项类。
/// </summary>
public class ClientOption
{
    internal static ClientOption Instance { get; } = new();

    /// <summary>
    /// 取得或设置WebApi地址。
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// 取得或设置Http请求异常处理方法。
    /// </summary>
    public Action<ErrorInfo> OnError { get; set; }

    /// <summary>
    /// 取得或设置客户端动态代理请求Api拦截器类型。
    /// </summary>
    public Func<Type, Type> InterceptorType { get; set; }

    /// <summary>
    /// 取得或设置客户端动态代理请求拦截器提供者。
    /// </summary>
    public Func<Type, object, object> InterceptorProvider { get; set; }
}

/// <summary>
/// 错误信息类。
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// 取得或设置Http请求地址。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置Http请求参数对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 取得或设置错误异常。
    /// </summary>
    public Exception Exception { get; set; }
}