namespace Known;

/// <summary>
/// 系统按钮信息类。
/// </summary>
public class ButtonInfo
{
    /// <summary>
    /// 取得或设置操作ID。
    /// </summary>
    [Form, Required]
    [Column(IsViewLink = true, Width = 150)]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置操作名称。
    /// </summary>
    [Form, Required]
    [Column(IsQuery = true, Width = 120)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置操作图标。
    /// </summary>
    [Required]
    [Column(Width = 100)]
    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(IconPicker))]
    [DisplayName("图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作样式，如：primary，danger等。
    /// </summary>
    [Column(Width = 100)]
    [Form(Type = nameof(FieldType.RadioList))]
    [Category("primary，danger")]
    [DisplayName("样式")]
    public string Style { get; set; }

    /// <summary>
    /// 取得或设置操作位置，如：Toolbar，Action。
    /// </summary>
    [Column(IsQueryAll = true)]
    [Form(Type = nameof(FieldType.CheckList))]
    [Category("Toolbar,Action")]
    [DisplayName("位置")]
    public string[] Position { get; set; }

    /// <summary>
    /// 将按钮信息转成操作信息。
    /// </summary>
    /// <returns></returns>
    public ActionInfo ToAction()
    {
        var info = new ActionInfo
        {
            Id = Id,
            Name = Name,
            Icon = Icon,
            Style = Style
        };
        if (Position != null && Position.Length > 0)
            info.Position = string.Join(",", Position);
        return info;
    }
}