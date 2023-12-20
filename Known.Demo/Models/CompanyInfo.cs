using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Demo.Models;

class CompanyInfo
{
    [Form(Row = 1, Column = 1, ReadOnly = true)]
    [DisplayName("企业编码"), Required]
    public string Code { get; set; }

    [Form(Row = 1, Column = 2)]
    [DisplayName("企业名称"), Required]
    public string Name { get; set; }

    [Form(Row = 2, Column = 1)]
    [DisplayName("英文名称")]
    public string NameEn { get; set; }

    [Form(Row = 2, Column = 2)]
    [DisplayName("社会信用代码")]
    public string SccNo { get; set; }

    [Form(Row = 3, Column = 1)]
    [DisplayName("中文地址")]
    public string Address { get; set; }

    [Form(Row = 4, Column = 1)]
    [DisplayName("英文地址")]
    public string AddressEn { get; set; }

    [Form(Row = 5, Column = 1)]
    [DisplayName("联系人")]
    public string Contact { get; set; }

    [Form(Row = 5, Column = 2)]
    [DisplayName("联系人电话")]
    public string Phone { get; set; }

    [Form(Row = 6, Column = 1, Type = "TextArea")]
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }
}