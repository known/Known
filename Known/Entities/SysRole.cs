namespace Known.Entities
{
    public class SysRole : EntityBase
    {
        [Column("名称", "", true, "1", "50")]
        public string Name { get; set; }

        [Column("状态", "", true)]
        public int Enabled { get; set; }

        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }
    }
}