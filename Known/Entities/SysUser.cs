namespace Known.Entities;

/// <summary>
/// 系统用户实体类。
/// </summary>
public class SysUser : EntityBase
{
    /// <summary>
    /// 取得或设置组织编码。
    /// </summary>
    [Column("组织编码", "", true, "1", "50", IsGrid = false)]
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Column("用户名", "", true, "1", "50")]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Column("密码", "", true, "1", "50")]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置姓名。
    /// </summary>
    [Column("姓名", "", true, "1", "50")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名。
    /// </summary>
    [Column("英文名", "", false, "1", "50")]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [Column("性别", "", true, "1", "50")]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置固定电话。
    /// </summary>
    [Column("固定电话", "", false, "1", "50")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置移动电话。
    /// </summary>
    [Column("移动电话", "", false, "1", "50")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置电子邮件。
    /// </summary>
    [Column("电子邮件", "", false, "1", "50")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column("状态", "", true)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置首次登录时间。
    /// </summary>
    [Column("首次登录时间", "", false)]
    public DateTime? FirstLoginTime { get; set; }

    /// <summary>
    /// 取得或设置首次登录IP。
    /// </summary>
    [Column("首次登录IP", "", false, "1", "50")]
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置最近登录时间。
    /// </summary>
    [Column("最近登录时间", "", false)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置最近登录IP。
    /// </summary>
    [Column("最近登录IP", "", false, "1", "50")]
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Column("类型", "", false, "1", "500")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置角色。
    /// </summary>
    [Column("角色", "", false, "1", "500")]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置数据。
    /// </summary>
    [Column("数据", "", false, IsGrid = false)]
    public string Data { get; set; }
}