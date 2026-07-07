namespace Known;

/// <summary>
/// 快捷键信息类。
/// </summary>
public class HotkeyInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public HotkeyInfo() { }

    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="key">快捷键。</param>
    /// <param name="invoke">调用组件的[JSInvokable]方法名。</param>
    public HotkeyInfo(string key, string invoke)
    {
        Key = key;
        Invoke = invoke;
    }

    /// <summary>
    /// 取得或设置快捷键。
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 取得或设置调用组件的[JSInvokable]方法名。
    /// </summary>
    public string Invoke { get; set; }
}
