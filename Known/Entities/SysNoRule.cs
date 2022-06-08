namespace Known.Entities
{
    /// <summary>
    /// 编号规则实体类。
    /// </summary>
    public class SysNoRule : EntityBase
    {
        /// <summary>
        /// 取得或设置编号。
        /// </summary>
        [Column("编号", "", true, "1", "50")]
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        [Column("名称", "", true, "1", "50")]
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        [Column("描述", "", false, "1", "500")]
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置规则。
        /// </summary>
        [Column("规则", "", false, "1", "4000")]
        public string RuleData { get; set; }
    }
}