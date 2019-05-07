using Known.Mapping;

namespace Known.Core.Entities
{
    public class Department : EntityBase
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
    }

    class DepartmentMapper : EntityMapper<Department>
    {
        public DepartmentMapper() :
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
