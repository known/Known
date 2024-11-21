namespace Known.Blazor;

/// <summary>
/// 打印组件呈现接口。
/// </summary>
public interface IPrintRenderer<T> where T : Microsoft.AspNetCore.Components.IComponent
{
    /// <summary>
    /// 设置打印组件参数。
    /// </summary>
    /// <typeparam name="TValue">组件参数对象类型。</typeparam>
    /// <param name="selector">组件参数属性选择表达式。</param>
    /// <param name="value">组件参数对象。</param>
    /// <returns>打印组件呈现对象。</returns>
    IPrintRenderer<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value);
}