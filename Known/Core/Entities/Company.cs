using Known.Mapping;

namespace Known.Core.Entities
{
    /// <summary>
    /// 公司实体类。
    /// </summary>
    public class Company : EntityBase
    {
        /// <summary>
        /// 取得或设置上级公司ID。
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 取得或设置公司名称。
        /// </summary>
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
