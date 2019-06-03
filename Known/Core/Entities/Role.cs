using Known.Mapping;

namespace Known.Core.Entities
{
    /// <summary>
    /// 应用程序角色实体类。
    /// </summary>
    public class Role : EntityBase
    {
        /// <summary>
        /// 取得或设置应用程序ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 取得或设置代码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }
    }

    class RoleMapper : EntityMapper<Role>
    {
        public RoleMapper() :
            base("t_plt_roles", "系统角色")
        {
            this.Property(p => p.AppId)
                .IsStringColumn("app_id", "APPID", 1, 50, true);

            this.Property(p => p.Code)
                .IsStringColumn("code", "代码", 1, 50, true);

            this.Property(p => p.Name)
                .IsStringColumn("name", "名称", 1, 50, true);

            this.Property(p => p.Description)
                .IsStringColumn("description", "描述", 1, 500);
        }
    }
}
