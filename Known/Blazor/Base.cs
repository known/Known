namespace Known.Blazor;

/// <summary>
/// 基本组件接口。
/// </summary>
public interface IBaseComponent : Microsoft.AspNetCore.Components.IComponent
{
    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置注入的UI服务实例。
    /// </summary>
    UIService UI { get; set; }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    Task<T> CreateServiceAsync<T>() where T : IService;

    /// <summary>
    /// 异步刷新组件。
    /// </summary>
    /// <returns></returns>
    Task RefreshAsync();
}