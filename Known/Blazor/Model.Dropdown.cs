namespace Known.Blazor;

/// <summary>
/// 下拉框配置模型信息类。
/// </summary>
public class DropdownModel
{
    /// <summary>
    /// 取得或设置元素样式类。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置触发类型（Click、ContextMenu、Hover、Focus），默认Hover。
    /// </summary>
    public string TriggerType { get; set; }

    /// <summary>
    /// 取得或设置下拉框图标提示信息。
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置下拉框显示图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置下拉框显示文本。
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 取得或设置下拉框显示文本加图标。
    /// </summary>
    public string TextIcon { get; set; }

    /// <summary>
    /// 取得或设置下拉框显示文本按钮。
    /// </summary>
    public string TextButton { get; set; }

    /// <summary>
    /// 取得或设置下拉框项目信息列表。
    /// </summary>
    public List<ActionInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置下拉框项目单击事件委托方法。
    /// </summary>
    public Func<ActionInfo, Task> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置显示内容模板。
    /// </summary>
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 取得或设置下拉框内容模板。
    /// </summary>
    public RenderFragment Overlay { get; set; }
}