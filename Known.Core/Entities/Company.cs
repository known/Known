using Known.Mapping;

namespace Known.Core.Entities
{
    [Table("t_plt_companies", "公司")]
    public class Company : EntityBase
    {
        [StringColumn("parent_id", "上级公司ID", 1, 50)]
        public string ParentId { get; set; }

        [StringColumn("name", "公司名称", 1, 50, true)]
        public string Name { get; set; }
    }
}
