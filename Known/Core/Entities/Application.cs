using Known.Mapping;

namespace Known.Core
{
    /// <summary>
    /// 应用程序实体类。
    /// </summary>
    public class Application : EntityBase
    {
        /// <summary>
        /// 取得或设置代码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
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
