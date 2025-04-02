namespace Known.Blazor;

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