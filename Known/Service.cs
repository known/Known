namespace Known;

/// <summary>
/// 前后端交互服务类接口。
/// </summary>
public interface IService
{
    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    Context Context { get; set; }
}

/// <summary>
/// 抽象客户端基类。
/// </summary>
/// <param name="http">HTTP客户端。</param>
public abstract class ClientBase(HttpClient http) : IService
{
    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    /// 取得HTTP客户端对象。
    /// </summary>
    public HttpClient Http => http;
}