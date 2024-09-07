namespace Known.Components;

/// <summary>
/// UI上下文级联值组件类。
/// </summary>
public class KContext : CascadingValue<UIContext>
{
    /// <summary>
    /// 构造函数，创建一个UI上下文级联值组件类的实例。
    /// </summary>
    public KContext()
    {
        IsFixed = true;
    }
}

/// <summary>
/// 路由级联值组件类。
/// </summary>
public class KRouteData : CascadingValue<RouteData>
{
    /// <summary>
    /// 构造函数，创建一个路由级联值组件类的实例。
    /// </summary>
    public KRouteData()
    {
        IsFixed = true;
    }
}