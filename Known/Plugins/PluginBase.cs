namespace Known.Plugins;

/// <summary>
/// 插件接口。
/// </summary>
public interface IPlugin : Microsoft.AspNetCore.Components.IComponent
{
    /// <summary>
    /// 取得或设置上级组件。
    /// </summary>
    BaseComponent Parent { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息。
    /// </summary>
    PluginInfo Plugin { get; set; }

    /// <summary>
    /// 显示插件配置界面。
    /// </summary>
    /// <param name="onConfig">配置委托。</param>
    void Config(Func<object, Task<Result>> onConfig);
}

/// <summary>
/// 插件组件基类。
/// </summary>
/// <typeparam name="TParam">插件配置参数类型。</typeparam>
public class PluginBase<TParam> : BaseComponent, IPlugin
{
    [CascadingParameter] private PluginPage Page { get; set; }
    private List<ActionInfo> Actions { get; } = [];

    /// <summary>
    /// 取得插件配置参数对象。
    /// </summary>
    protected TParam Parameter { get; set; }

    /// <summary>
    /// 取得或设置插件是否可以拖拽。
    /// </summary>
    protected bool Draggable { get; set; }

    /// <summary>
    /// 取得或设置上级组件。
    /// </summary>
    public BaseComponent Parent { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息。
    /// </summary>
    [Parameter] public PluginInfo Plugin { get; set; }

    /// <summary>
    /// 显示插件配置界面。
    /// </summary>
    /// <param name="onConfig">配置委托。</param>
    public virtual void Config(Func<object, Task<Result>> onConfig) { }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AddAction("delete", "删除", OnDelete);
    }

    /// <summary>
    /// 构建插件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        Parameter = Utils.FromJson<TParam>(Plugin?.Setting);
        builder.Component<PluginPanel>()
               .Set(c => c.Draggable, Draggable)
               .Set(c => c.Actions, Actions)
               .Set(c => c.ChildContent, BuildPlugin)
               .Build();
    }

    /// <summary>
    /// 构建插件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildPlugin(RenderTreeBuilder builder) { }

    /// <summary>
    /// 添加插件配置下拉菜单项。
    /// </summary>
    /// <param name="icon">图标。</param>
    /// <param name="name">名称。</param>
    /// <param name="onClick">单击事件委托。</param>
    /// <returns>下拉菜单项。</returns>
    protected ActionInfo AddAction(string icon, string name, Action onClick)
    {
        var info = new ActionInfo
        {
            Icon = icon,
            Name = name,
            OnClick = this.Callback<MouseEventArgs>(e => onClick?.Invoke())
        };
        Actions.Add(info);
        return info;
    }

    /// <summary>
    /// 异步保存插件设置参数。
    /// </summary>
    /// <param name="parameter">插件参数对象。</param>
    /// <returns></returns>
    protected Task<Result> SaveParameterAsync(TParam parameter)
    {
        var plugin = Page.Menu.Plugins.FirstOrDefault(p => p.Id == Plugin.Id);
        if (plugin == null)
            return Result.ErrorAsync("插件不存在！");

        plugin.Setting = Utils.ToJson(parameter);
        return Platform.SaveMenuAsync(Page.Menu);
    }

    private void OnDelete()
    {
        UI.Confirm("确定要删除插件？", async () =>
        {
            Page.Menu.Plugins.Remove(Plugin);
            await Platform.SaveMenuAsync(Page.Menu);
            await Page.StateChangedAsync();
        });
    }
}