namespace Known.Core.Entities
{
    public class SysRole : EntityBase
    {
        [Column("名称", "", true, "1", "50")]
        public string Name { get; set; }
        [Column("状态", "", true)]
        public int Enabled { get; set; }
        public string Note { get; set; }
    }
}
