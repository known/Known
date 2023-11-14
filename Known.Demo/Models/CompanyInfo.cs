using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Demo.Models;

class CompanyInfo
{
    [DisplayName("企业编码"), Required]
    public string Code { get; set; }

    [DisplayName("企业名称"), Required]
    public string Name { get; set; }

    [DisplayName("英文名称")]
    public string NameEn { get; set; }

    [DisplayName("社会信用代码")]
    public string SccNo { get; set; }

    [DisplayName("中文地址")]
    public string Address { get; set; }

    [DisplayName("英文地址")]
    public string AddressEn { get; set; }

    [DisplayName("联系人")]
    public string Contact { get; set; }

    [DisplayName("联系人电话")]
    public string Phone { get; set; }

    [DisplayName("备注")]
    public string Note { get; set; }
}