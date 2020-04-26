namespace Known.Core.Entities
{
    public class SysDictionary : EntityBase
    {
        public string Category { get; set; }
        public string CategoryName { get; set; }
        [Column("代码", "", true, "1", "50")]
        public string Code { get; set; }
        [Column("名称", "", false, "1", "50")]
        public string Name { get; set; }
        [Column("顺序", "", true)]
        public int Sort { get; set; }
        [Column("状态", "", true)]
        public int Enabled { get; set; }
        public string Note { get; set; }
    }
}
