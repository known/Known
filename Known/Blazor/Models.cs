namespace Known.Blazor;

/// <summary>
/// 加载旋转组件模型类。
/// </summary>
public class SpinModel
{
    /// <summary>
    /// 取得或设置是否旋转。
    /// </summary>
    public bool Spinning { get; set; }

    /// <summary>
    /// 取得或设置提示文本。
    /// </summary>
    public string Tip { get; set; }

    /// <summary>
    /// 取得或设置组件内部内容。
    /// </summary>
    public RenderFragment Content { get; set; }
}

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
    public Func<Task> OnClose { get; set; }

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

/// <summary>
/// 下拉框配置模型信息类。
/// </summary>
public class DropdownModel
{
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
    public Action<ActionInfo> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置下拉框内容模板。
    /// </summary>
    public RenderFragment Overlay { get; set; }
}

/// <summary>
/// 输入组件配置模型信息类。
/// </summary>
/// <typeparam name="TValue">输入组件数据类型。</typeparam>
public class InputModel<TValue>
{
    /// <summary>
    /// 取得或设置输入组件是否可用。
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// 取得或设置CheckBox输入组件是否半选。
    /// </summary>
    public bool Indeterminate { get; set; }

    /// <summary>
    /// 取得或设置CheckBox输入组件文本。
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 取得或设置输入组件文本占位符。
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// 取得或设置输入组件绑定的数据值。
    /// </summary>
    public TValue Value { get; set; }

    /// <summary>
    /// 取得或设置输入组件数据改变时回调委托。
    /// </summary>
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// 取得或设置输入组件关联的代码表列表。
    /// </summary>
    public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 取得或设置多行文本输入组件行数。
    /// </summary>
    public uint Rows { get; set; }
}