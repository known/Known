namespace Known.Core.Entities
{
    public class SysModule : EntityBase
    {
        [Column("上级模块", "", false, "1", "50")]
        public string ParentId { get; set; }

        [Column("类型", "", true, "1", "50")]
        public string Type { get; set; }

        [Column("编码", "", true, "1", "50")]
        public string Code { get; set; }

        [Column("名称", "", true, "1", "50")]
        public string Name { get; set; }

        [Column("图标", "", true, "1", "50")]
        public string Icon { get; set; }

        [Column("URL", "", false, "1", "200")]
        public string Url { get; set; }

        [Column("目标", "", false, "1", "50")]
        public string Target { get; set; }

        [Column("顺序", "", true)]
        public int Sort { get; set; }

        [Column("状态", "", true)]
        public int Enabled { get; set; }

        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }
    }
}
