namespace Known;

/// <summary>
/// 栏位信息类。
/// </summary>
public partial class ColumnInfo
{
    /// <summary>
    /// 构造函数，创建一个栏位信息类的实例。
    /// </summary>
    public ColumnInfo() { }

    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置栏位提示文字。
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见，默认True。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得栏位关联的对象属性。
    /// </summary>
    public PropertyInfo Property { get; internal set; }
}