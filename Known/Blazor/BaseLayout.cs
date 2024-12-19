namespace Known.Blazor;

/// <summary>
/// 基础模板组件类。
/// </summary>
public class BaseLayout : BaseComponent
{
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

    /// <summary>
    /// 重新加载当前页面，如果是多标签，则刷新当前标签页。
    /// </summary>
    public virtual void ReloadPage() { }

    internal virtual void ToggleSide(bool collapsed) { }
    internal virtual void AddMenuItem(MenuInfo item) { }
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

/// <summary>
/// 管理后台模板页类。
/// </summary>
public class AdminLayout : LayoutComponentBase
{
    /// <summary>
    /// 呈现模板内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (UIConfig.AdminBody != null)
        {
            builder.Component<KLayout>()
                   .Set(c => c.IsAdmin, true)
                   .Set(c => c.ChildContent, b => UIConfig.AdminBody.Invoke(b, Body))
                   .Build();
        }
        else
        {
            builder.Component<KLayout>()
                   .Set(c => c.IsAdmin, true)
                   .Set(c => c.ChildContent, Body)
                   .Build();
        }
    }
}