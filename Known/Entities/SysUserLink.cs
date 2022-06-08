namespace Known.Entities
{
    /// <summary>
    /// 用户链接实体类。
    /// </summary>
    public class SysUserLink : EntityBase
    {
        /// <summary>
        /// 取得或设置用户名。
        /// </summary>
        [Column("用户名", "", true, "1", "50")]
        public string UserName { get; set; }

        /// <summary>
        /// 取得或设置类型（快捷方式，常用链接）。
        /// </summary>
        [Column("类型", "", true, "1", "50")]
        public string Type { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        [Column("名称", "", true, "1", "50")]
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置地址。
        /// </summary>
        [Column("地址", "", true, "1", "200")]
        public string Address { get; set; }
    }
}