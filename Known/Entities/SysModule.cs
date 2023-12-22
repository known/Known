using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统模块实体类。
/// </summary>
public class SysModule : EntityBase
{
    public SysModule()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    [DisplayName("上级")]
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [DisplayName("代码")]
    [Required(ErrorMessage = "代码不能为空！")]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [DisplayName("图标")]
    [MaxLength(50)]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    [DisplayName("描述")]
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    [DisplayName("类型")]
    [Required(ErrorMessage = "请选择模块类型！")]
    [MaxLength(250)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [DisplayName("顺序")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    [DisplayName("可用")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置实体设置。
    /// </summary>
    [DisplayName("实体设置")]
    public string EntityData { get; set; }

    /// <summary>
    /// 取得或设置页面设置。
    /// </summary>
    [DisplayName("页面设置")]
    public string PageData { get; set; }

    /// <summary>
    /// 取得或设置表单设置。
    /// </summary>
    [DisplayName("表单设置")]
    public string FormData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    public virtual string ParentName { get; set; }
    public virtual bool IsMoveUp { get; set; }

    internal virtual PageInfo Page
    {
        get { return Utils.FromJson<PageInfo>(PageData) ?? new(); }
        set { PageData = Utils.ToJson(value); }
    }

    internal virtual FormInfo Form
    {
        get { return Utils.FromJson<FormInfo>(FormData) ?? new(); }
        set { FormData = Utils.ToJson(value); }
    }

    internal virtual List<string> Buttons { get; set; }
    internal virtual List<string> Actions { get; set; }
    internal virtual List<PageColumnInfo> Columns { get; set; }

    internal void LoadData()
    {
        Config.PageButtons.TryGetValue(Code, out List<string> buttons);
        Config.PageActions.TryGetValue(Code, out List<string> actions);
        Buttons = buttons;
        Actions = actions;
        Columns = Page?.Columns;
    }
}