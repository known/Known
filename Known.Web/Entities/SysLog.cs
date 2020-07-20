namespace Known.Web.Entities
{
    public class SysLog : EntityBase
    {
        public string Type { get; set; }
        public string Target { get; set; }
        public string Content { get; set; }
    }
}