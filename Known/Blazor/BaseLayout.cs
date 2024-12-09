namespace Known.Blazor;

/// <summary>
/// 基础模板组件类。
/// </summary>
public class BaseLayout : BaseComponent
{
    /// <summary>
    /// 取得上下文当前菜单信息实例。
    /// </summary>
    internal MenuInfo CurrentMenu => Context?.Current;

    /// <summary>
    /// 导航到指定菜单对应的页面。
    /// </summary>
    /// <param name="item">跳转的菜单对象。</param>
    public void NavigateTo(MenuInfo item) => Navigation?.NavigateTo(item);

    /// <summary>
    /// 返回到上一个页面。
    /// </summary>
    public void Back()
    {
        if (CurrentMenu == null || string.IsNullOrWhiteSpace(CurrentMenu.BackUrl))
            return;

        Navigation?.NavigateTo(CurrentMenu.BackUrl);
    }

    /// <summary>
    /// 异步显示加载提示框虚方法。
    /// </summary>
    /// <param name="text">加载提示信息。</param>
    /// <param name="action">异步加载方法的委托。</param>
    /// <returns></returns>
    public virtual Task ShowSpinAsync(string text, Func<Task> action) => Task.CompletedTask;

    /// <summary>
    /// 异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns></returns>
    public virtual Task SignInAsync(UserInfo user) => Task.CompletedTask;

    /// <summary>
    /// 异步注销，用户安全退出系统。
    /// </summary>
    /// <returns></returns>
    public virtual Task SignOutAsync() => Task.CompletedTask;
}

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : LayoutComponentBase
{
    /// <summary>
    /// 呈现空模板组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-wrapper", () => builder.Fragment(Body));
    }
}