namespace Known.Components;

/// <summary>
/// 按钮组件类。
/// </summary>
public class KButton : BaseComponent
{
    /// <summary>
    /// 取得或设置按钮图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置按钮样式。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置按钮单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// 取得或设置操作按钮信息。
    /// </summary>
    [Parameter] public ActionInfo Action { get; set; }

    /// <summary>
    /// 呈现按钮组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        Action ??= new ActionInfo
        {
            Id = Id,
            Name = Name,
            Icon = Icon,
            Style = Style,
            Enabled = Enabled,
            Visible = Visible,
            OnClick = OnClick
        };
        UI.BuildButton(builder, Action);
    }
}