namespace Known.Core.Entities
{
    public class SysRole : EntityBase
    {
        public string Name { get; set; }
        public int Enabled { get; set; }
        public string Note { get; set; }
    }
}
