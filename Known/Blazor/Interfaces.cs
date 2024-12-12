namespace Known.Blazor;

/// <summary>
/// 无代码页面接口。
/// </summary>
public interface IAutoPage
{
    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();
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