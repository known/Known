using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.WorkFlows;

/// <summary>
/// 流程步骤实体类。
/// </summary>
public class SysFlowStep : EntityBase
{
    /// <summary>
    /// 取得或设置流程代码。
    /// </summary>
    [DisplayName("流程代码")]
    [Required]
    [MaxLength(50)]
    public string FlowCode { get; set; }

    /// <summary>
    /// 取得或设置流程名称。
    /// </summary>
    [DisplayName("流程名称")]
    [Required]
    [MaxLength(50)]
    public string FlowName { get; set; }

    /// <summary>
    /// 取得或设置步骤代码。
    /// </summary>
    [DisplayName("步骤代码")]
    [Required]
    [MaxLength(50)]
    public string StepCode { get; set; }

    /// <summary>
    /// 取得或设置步骤名称。
    /// </summary>
    [DisplayName("步骤名称")]
    [Required]
    [MaxLength(50)]
    public string StepName { get; set; }

    /// <summary>
    /// 取得或设置步骤类型。
    /// </summary>
    [DisplayName("步骤类型")]
    [Required]
    [MaxLength(50)]
    public string StepType { get; set; }

    /// <summary>
    /// 取得或设置操作人。
    /// </summary>
    [DisplayName("操作人")]
    [MaxLength(500)]
    public string OperateBy { get; set; }

    /// <summary>
    /// 取得或设置操作角色。
    /// </summary>
    [DisplayName("操作角色")]
    [MaxLength(500)]
    public string OperateRole { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置结果数据。
    /// </summary>
    [DisplayName("结果数据")]
    public string ResultData { get; set; }

    /// <summary>
    /// 取得或设置设计数据。
    /// </summary>
    [DisplayName("设计数据")]
    public string DesignData { get; set; }
}