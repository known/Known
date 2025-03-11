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
/// 扩展Ant表单接口。
/// </summary>
public interface IAntForm
{
    /// <summary>
    /// 取得表单是否查看模式。
    /// </summary>
    bool IsView { get; }
}