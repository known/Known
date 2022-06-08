using System;

namespace Known.Entities
{
    /// <summary>
    /// 工作流日志实体类。
    /// </summary>
    public class SysFlowLog : EntityBase
    {
        /// <summary>
        /// 取得或设置业务ID。
        /// </summary>
        [Column("业务ID", "", true, "1", "50")]
        public string BizId { get; set; }

        /// <summary>
        /// 取得或设置步骤。
        /// </summary>
        [Column("步骤", "", true, "1", "50")]
        public string StepName { get; set; }

        /// <summary>
        /// 取得或设置执行人ID。
        /// </summary>
        [Column("执行人ID", "", true, "1", "50")]
        public string ExecuteById { get; set; }

        /// <summary>
        /// 取得或设置执行人。
        /// </summary>
        [Column("执行人", "", true, "1", "50")]
        public string ExecuteBy { get; set; }

        /// <summary>
        /// 取得或设置执行时间。
        /// </summary>
        [Column("执行时间", "", true)]
        public DateTime ExecuteTime { get; set; }

        /// <summary>
        /// 取得或设置执行结果（通过、退回、撤回）。
        /// </summary>
        [Column("执行结果", "", true, "1", "50")]
        public string Result { get; set; }

        /// <summary>
        /// 取得或设置执行内容。
        /// </summary>
        [Column("执行内容", "", false, "1", "1000")]
        public string Note { get; set; }
    }
}