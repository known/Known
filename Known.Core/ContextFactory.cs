namespace Known.Core;

/// <summary>
/// 系统上下文工厂类。
/// </summary>
public class ContextFactory
{
    /// <summary>
    /// 创建系统上下文请求对象。
    /// </summary>
    /// <param name="context">Http上下文对象。</param>
    /// <returns>系统上下文请求对象。</returns>
    public static IRequest CreateRequest(HttpContext context)
    {
        return new WebRequest(context);
    }

    /// <summary>
    /// 创建系统上下文响应对象。
    /// </summary>
    /// <param name="context">Http上下文对象。</param>
    /// <returns>系统上下文响应对象。</returns>
    public static IResponse CreateResponse(HttpContext context)
    {
        return new WebResponse(context);
    }
}