using Known.Mapping;

namespace Known.Core.Entities
{
    public class Company : EntityBase
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
    }

    class CompanyMapper : EntityMapper<Company>
    {
        public CompanyMapper() :
            base("t_plt_companies", "公司")
        {
            this.Property(p => p.ParentId)
                .IsStringColumn("parent_id", "上级公司ID", 1, 50);

            this.Property(p => p.Name)
                .IsStringColumn("name", "公司名称", 1, 50, true);
        }
    }
}
