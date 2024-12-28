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

    /// <summary>
    /// 取得插件配置参数对象。
    /// </summary>
    protected TParam Parameter { get; private set; }

    /// <summary>
    /// 取得插件配置下拉菜单项列表。
    /// </summary>
    protected List<ActionInfo> Actions { get; } = [];

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
        Parameter = Utils.FromJson<TParam>(Plugin?.Setting);
        Actions.Add(new ActionInfo
        {
            Id = "Delete",
            Icon = "delete",
            Name = "删除",
            OnClick = this.Callback<MouseEventArgs>(OnDelete)
        });
    }

    /// <summary>
    /// 构建插件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
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

    private async Task OnDelete(MouseEventArgs args)
    {
        Page.Menu.Plugins.Remove(Plugin);
        await Platform.SaveMenuAsync(Page.Menu);
        await Page.StateChangedAsync();
    }
}