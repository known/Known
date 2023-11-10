namespace Known.Entities;

/// <summary>
/// 系统角色实体类。
/// </summary>
public class SysRole : EntityBase
{
    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column("名称", "", true, "1", "50", IsGrid = true, IsQuery = true, IsForm = true)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column("状态", "", true, IsGrid = true, IsForm = true)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500", IsGrid = true, IsForm = true)]
    public string Note { get; set; }
}