namespace Known.Core.Entities
{
    public class SysOrganization : EntityBase
    {
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string Note { get; set; }
    }
}
