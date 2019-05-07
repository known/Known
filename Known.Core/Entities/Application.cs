using Known.Mapping;

namespace Known.Core.Entities
{
    public class Application : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
    }

    class ApplicationMapper : EntityMapper<Application>
    {
        public ApplicationMapper() :
            base("t_plt_applications", "应用程序")
        {
            this.Property(p => p.Code)
                .IsStringColumn("code", "代码", 1, 50, true);

            this.Property(p => p.Name)
                .IsStringColumn("name", "名称", 1, 50, true);

            this.Property(p => p.Version)
                .IsStringColumn("version", "版本", 1, 50, true);

            this.Property(p => p.Description)
                .IsStringColumn("description", "描述", 1, 500);
        }
    }
}
