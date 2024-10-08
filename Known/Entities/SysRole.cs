﻿namespace Known.Entities;

/// <summary>
/// 系统角色实体类。
/// </summary>
public class SysRole : EntityBase
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SysRole()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置角色关联的模块列表。
    /// </summary>
    public virtual List<SysModule> Modules { get; set; }

    /// <summary>
    /// 取得或设置角色关联的菜单ID列表。
    /// </summary>
    public virtual List<string> MenuIds { get; set; }
}