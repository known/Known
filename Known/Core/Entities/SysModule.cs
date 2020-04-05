namespace Known.Core.Entities
{
    public class SysModule : EntityBase
    {
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int Sort { get; set; }
        public int Enabled { get; set; }
    }
}
