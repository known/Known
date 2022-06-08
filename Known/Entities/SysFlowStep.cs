/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

namespace Known.Entities
{
    /// <summary>
    /// 工作流步骤实体类。
    /// </summary>
    public class SysFlowStep : EntityBase
    {
        /// <summary>
        /// 取得或设置流程代码。
        /// </summary>
        [Column("流程代码", "", true, "1", "50")]
        public string FlowCode { get; set; }

        /// <summary>
        /// 取得或设置流程名称。
        /// </summary>
        [Column("流程名称", "", true, "1", "50")]
        public string FlowName { get; set; }

        /// <summary>
        /// 取得或设置步骤代码。
        /// </summary>
        [Column("步骤代码", "", true, "1", "50")]
        public string StepCode { get; set; }

        /// <summary>
        /// 取得或设置步骤名称。
        /// </summary>
        [Column("步骤名称", "", true, "1", "50")]
        public string StepName { get; set; }

        /// <summary>
        /// 取得或设置步骤类型。
        /// </summary>
        [Column("步骤类型", "", true, "1", "50")]
        public string StepType { get; set; }

        /// <summary>
        /// 取得或设置操作人ID。
        /// </summary>
        [Column("操作人ID", "", false, "1", "50")]
        public string OperateBy { get; set; }

        /// <summary>
        /// 取得或设置操作人。
        /// </summary>
        [Column("操作人", "", false, "1", "50")]
        public string OperateByName { get; set; }

        /// <summary>
        /// 取得或设置备注。
        /// </summary>
        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }

        /// <summary>
        /// 取得或设置X坐标。
        /// </summary>
        [Column("X坐标", "", false)]
        public int? X { get; set; }

        /// <summary>
        /// 取得或设置Y坐标。
        /// </summary>
        [Column("Y坐标", "", false)]
        public int? Y { get; set; }

        /// <summary>
        /// 取得或设置圆角。
        /// </summary>
        [Column("圆角", "", false)]
        public int? IsRound { get; set; }

        /// <summary>
        /// 取得或设置箭头。
        /// </summary>
        [Column("箭头", "", false, "1", "4000")]
        public string Arrows { get; set; }
    }
}