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
    [Column]
    [DisplayName("上级")]
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, IsViewLink = true)]
    [DisplayName("代码")]
    [Required(ErrorMessage = "代码不能为空！")]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [Column(IsForm = true)]
    [DisplayName("图标")]
    [MaxLength(50)]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("描述")]
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    [Column]
    [DisplayName("目标")]
    [MaxLength(250)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("顺序")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("可用")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置按钮。
    /// </summary>
    [Column]
    [DisplayName("按钮")]
    public string ButtonData { get; set; }

    /// <summary>
    /// 取得或设置操作。
    /// </summary>
    [Column]
    [DisplayName("操作")]
    public string ActionData { get; set; }

    /// <summary>
    /// 取得或设置栏位。
    /// </summary>
    [Column]
    [DisplayName("栏位")]
    public string ColumnData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    public virtual bool IsMoveUp { get; set; }
    public virtual List<string> Buttons => ButtonData?.Split(",").ToList();
    public virtual List<string> Actions => ActionData?.Split(",").ToList();
    public virtual List<ColumnInfo> Columns => Utils.FromJson<List<ColumnInfo>>(ColumnData);
}