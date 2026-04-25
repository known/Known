using AntDesign;

namespace Known.Blazor;

/// <summary>
/// 抽屉配置模型信息类。
/// </summary>
public class DrawerModel
{
    /// <summary>
    /// 取得或设置抽屉CSS类名。
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 取得或设置抽屉标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置抽屉宽度。
    /// </summary>
    public string Width { get; set; }

    /// <summary>
    /// 取得或设置是否显示关闭按钮，默认为true。
    /// </summary>
    public bool Closable { get; set; } = true;

    /// <summary>
    /// 取得或设置是否点击遮罩层关闭抽屉，默认为true。
    /// </summary>
    public bool MaskClosable { get; set; } = true;

    /// <summary>
    /// 取得或设置抽屉打开位置，默认为右侧。
    /// </summary>
    public DrawerPlacement Placement { get; set; } = DrawerPlacement.Right;

    /// <summary>
    /// 取得或设置抽屉内容呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }
}