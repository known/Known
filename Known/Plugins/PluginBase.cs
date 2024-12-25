namespace Known.Plugins;

/// <summary>
/// 插件接口。
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// 取得或设置上级组件。
    /// </summary>
    BaseComponent Parent { get; set; }

    /// <summary>
    /// 显示插件配置界面。
    /// </summary>
    /// <param name="onConfig">配置委托。</param>
    void Config(Func<object, Task<Result>> onConfig);

    /// <summary>
    /// 呈现插件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">插件信息。</param>
    void Render(RenderTreeBuilder builder, PluginInfo info);
}

/// <summary>
/// 插件基类。
/// </summary>
public class PluginBase
{
    /// <summary>
    /// 取得或设置上级组件。
    /// </summary>
    public BaseComponent Parent { get; set; }
}