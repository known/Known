using Known.Mapping;

namespace Known.Core.Entities
{
    [Table("t_plt_departments", "部门")]
    public class Department : EntityBase
    {
        [StringColumn("parent_id", "上级部门ID", 1, 50)]
        public string ParentId { get; set; }

        [StringColumn("name", "部门名称", 1, 50, true)]
        public string Name { get; set; }

        [StringColumn("parent_id", "部门主管ID", 1, 50)]
        public string ManagerId { get; set; }
    }
}
