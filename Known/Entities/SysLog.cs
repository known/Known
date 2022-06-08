namespace Known.Entities
{
    public class SysLog : EntityBase
    {
        [Column("Type", "", true, "1", "50")]
        public string Type { get; set; }
        [Column("Target", "", true, "1", "50")]
        public string Target { get; set; }
        [Column("Content")]
        public string Content { get; set; }
    }
}