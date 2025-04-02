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