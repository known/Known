namespace Known.Blazor;

/// <summary>
/// 无代码自动页面接口。
/// </summary>
public interface IAutoPage
{
    /// <summary>
    /// 取得或设置UI上下文对象实例。
    /// </summary>
    UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置自动页面ID。
    /// </summary>
    string PageId { get; set; }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    Task<T> CreateServiceAsync<T>() where T : IService;
}

/// <summary>
/// 容器组件接口。
/// </summary>
public interface IComContainer
{
    /// <summary>
    /// 取得或设置是否只读查看模式。
    /// </summary>
    bool IsView { get; set; }
}

/// <summary>
/// 授权组件接口。
/// </summary>
public interface IAuthComponent
{
    /// <summary>
    /// 验证组件是否已经授权。
    /// </summary>
    /// <returns></returns>
    ActiveInfo ValidateAuth();
}