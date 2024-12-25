namespace Known.Components;

/// <summary>
/// 插件面板组件类。
/// </summary>
public class PluginPanel : BaseComponent
{
    /// <summary>
    /// 取得或设置是否可以移动。
    /// </summary>
    [Parameter] public bool CanMove { get; set; }

    /// <summary>
    /// 取得或设置操作信息列表。
    /// </summary>
    [Parameter] public List<ActionInfo> Actions { get; set; }

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (UIConfig.IsEditMode)
        {
            builder.Div().Class("kui-plugin").Child(() =>
            {
                builder.Div().Class("action").Child(() => BuildAction(builder));
                builder.Fragment(ChildContent);
            });
        }
        else
        {
            builder.Fragment(ChildContent);
        }
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        if (CanMove)
            builder.Icon("fullscreen");
        var model = new DropdownModel
        {
            Icon = "menu",
            Items = Actions,
            TriggerType = "Click",
            OnItemClick = OnItemClickAsync
        };
        builder.Dropdown(model);
    }

    private Task OnItemClickAsync(ActionInfo info)
    {
        if (!info.OnClick.HasDelegate)
            return Task.CompletedTask;

        return info.OnClick.InvokeAsync();
    }
}