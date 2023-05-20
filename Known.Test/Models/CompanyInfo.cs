namespace Known.Test.Models;

public class CompanyInfo
{
    [Column("企业编码", true)] public string Code { get; set; }
    [Column("企业名称", true)] public string Name { get; set; }
    [Column("英文名称")] public string NameEn { get; set; }
    [Column("社会信用代码")] public string SccNo { get; set; }
    [Column("中文地址")] public string Address { get; set; }
    [Column("英文地址")] public string AddressEn { get; set; }
    [Column("联系人")] public string Contact { get; set; }
    [Column("联系人电话")] public string Phone { get; set; }
    [Column("备注")] public string Note { get; set; }
}