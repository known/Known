using Known.Mapping;

namespace Known.Core.Entities
{
    /// <summary>
    /// 应用程序实体类。
    /// </summary>
    public class TApplication : EntityBase
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
        public string VersionNo { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }
    }

    class TApplicationMapper : EntityMapper<TApplication>
    {
        public TApplicationMapper() :
            base("t_plt_applications", "应用程序")
        {
            this.Property(p => p.Code)
                .IsStringColumn("code", "代码", 1, 50, true);

            this.Property(p => p.Name)
                .IsStringColumn("name", "名称", 1, 50, true);

            this.Property(p => p.VersionNo)
                .IsStringColumn("version_no", "版本", 1, 50, true);

            this.Property(p => p.Description)
                .IsStringColumn("description", "描述", 1, 500);
        }
    }
}
