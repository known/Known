using Known.Mapping;

namespace Known.Web.Entities
{
    /// <summary>
    /// 部门实体类。
    /// </summary>
    public class TDepartment : EntityBase
    {
        /// <summary>
        /// 取得或设置上级部门ID。
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 取得或设置部门名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置部门主管ID。
        /// </summary>
        public string ManagerId { get; set; }
    }

    class TDepartmentMapper : EntityMapper<TDepartment>
    {
        public TDepartmentMapper() :
            base("t_plt_departments", "部门")
        {
            this.Property(p => p.ParentId)
                .IsStringColumn("parent_id", "上级部门ID", 1, 50);

            this.Property(p => p.Name)
                .IsStringColumn("name", "部门名称", 1, 50, true);

            this.Property(p => p.ManagerId)
                .IsStringColumn("parent_id", "部门主管ID", 1, 50);
        }
    }
}
