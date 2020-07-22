using System.Web.UI.WebControls;

namespace Known.Web
{
    public class DomainInfo
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Fields { get; set; }
    }

    public class FieldInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Length { get; set; }
        public string Control { get; set; }
    }
}
