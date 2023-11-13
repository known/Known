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
    [Column("上级", "", false, "1", "50")]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Column("代码", "", false, "1", "50", IsGrid = true, IsForm = true, IsViewLink = true)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column("名称", "", true, "1", "50", IsGrid = true, IsForm = true)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [Column("图标", "", false, "1", "50", IsForm = true)]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    [Column("描述", "", false, "1", "200", IsGrid = true, IsForm = true)]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    [Column("目标", "", false, "1", "250")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Column("顺序", IsGrid = true, IsForm = true)]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    [Column("可用", IsGrid = true, IsForm = true)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置按钮。
    /// </summary>
    [Column("按钮")]
    public string ButtonData { get; set; }

    /// <summary>
    /// 取得或设置操作。
    /// </summary>
    [Column("操作")]
    public string ActionData { get; set; }

    /// <summary>
    /// 取得或设置栏位。
    /// </summary>
    [Column("栏位")]
    public string ColumnData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500", IsGrid = true, IsForm = true)]
    public string Note { get; set; }

    public virtual bool IsMoveUp { get; set; }
    public virtual List<string> Buttons => ButtonData?.Split(",").ToList();
    public virtual List<string> Actions => ActionData?.Split(",").ToList();
    public virtual List<ColumnInfo> Columns => Utils.FromJson<List<ColumnInfo>>(ColumnData);
}