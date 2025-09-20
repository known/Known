namespace Known.Components;

/// <summary>
/// 强度组件。
/// </summary>
public partial class KStrength
{
    private string ClassName => CssBuilder.Default("kui-strength").AddClass(Value).BuildClass();

    /// <summary>
    /// 取得或设置强度（weak，medium，strong）。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置提示文本。
    /// </summary>
    [Parameter] public string Tips { get; set; }
}