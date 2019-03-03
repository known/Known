using Known.Mapping;

namespace Known.Core.Entities
{
    [Table("t_plt_applications", "应用程序")]
    public class Application : EntityBase
    {
        [StringColumn("code", "代码", 1, 50, true)]
        public string Code { get; set; }

        [StringColumn("name", "名称", 1, 50, true)]
        public string Name { get; set; }

        [StringColumn("version", "版本", 1, 50, true)]
        public string Version { get; set; }

        [StringColumn("description", "描述", 1, 500)]
        public string Description { get; set; }
    }
}
