namespace Known.Blazor;

/// <summary>
/// 对话框配置模型信息类。
/// </summary>
public class DialogModel
{
    /// <summary>
    /// 取得或设置对话框CSS类名。
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 取得或设置对话框CSS样式。
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// 取得或设置对话框标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置对话框是否可关闭，默认是。
    /// </summary>
    public bool Closable { get; set; } = true;

    /// <summary>
    /// 取得或设置对话框是否可拖动，默认是。
    /// </summary>
    public bool Draggable { get; set; } = true;

    /// <summary>
    /// 取得或设置对话框是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// 取得或设置对话框是否显示最大化按钮。
    /// </summary>
    public bool Maximizable { get; set; }

    /// <summary>
    /// 取得或设置对话框是否默认最大化显示。
    /// </summary>
    public bool DefaultMaximized { get; set; }

    /// <summary>
    /// 取得或设置对话框宽度。
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// 取得或设置对话框确定按钮事件委托方法。
    /// </summary>
    public Func<Task> OnOk { get; set; }

    /// <summary>
    /// 取得或设置对话框关闭委托。
    /// </summary>
    internal Func<Task> OnClose { get; set; }

    /// <summary>
    /// 取得或设置对话框关闭后调用的委托。
    /// </summary>
    public Action OnClosed { get; set; }

    /// <summary>
    /// 取得或设置对话框内容呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }

    /// <summary>
    /// 取得或设置对话框自定义底部模板。
    /// </summary>
    public RenderFragment Footer { get; set; }

    /// <summary>
    /// 取得或设置表单对话框底部左侧自定义组件。
    /// </summary>
    public RenderFragment FooterLeft { get; set; }

    /// <summary>
    /// 取得表单操作按钮信息列表，用于扩展表单底部按钮。
    /// </summary>
    public List<ActionInfo> Actions { get; } = [];

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="onClick">点击事件委托。</param>
    public void AddAction(string idOrName, EventCallback<MouseEventArgs> onClick)
    {
        Actions?.Add(new ActionInfo(idOrName) { OnClick = onClick });
    }

    /// <summary>
    /// 异步关闭对话框。
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();

        OnClosed?.Invoke();
    }
}