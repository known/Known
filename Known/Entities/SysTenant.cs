namespace Known.Entities;

/// <summary>
/// 租户信息实体类。
/// </summary>
public class SysTenant : EntityBase
{
    /// <summary>
    /// 取得或设置账号。
    /// </summary>
    [Column("账号", "", true, "1", "50")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column("名称", "", true, "1", "50")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column("状态", "", true, "1", "50")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置用户数量。
    /// </summary>
    [Column("用户数量", "", true)]
    public int UserCount { get; set; }

    /// <summary>
    /// 取得或设置单据数量。
    /// </summary>
    [Column("单据数量", "", true)]
    public int BillCount { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false)]
    public string Note { get; set; }
}