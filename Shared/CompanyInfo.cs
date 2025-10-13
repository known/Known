namespace Known;

/// <summary>
/// 企业信息类。
/// </summary>
public class CompanyInfo
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [Required]
    [Form(Row = 1, Column = 1, ReadOnly = true)]
    [DisplayName("企业编码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [Required]
    [Form(Row = 1, Column = 2)]
    [DisplayName("企业名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置企业英文名。
    /// </summary>
    [Form(Row = 2, Column = 1)]
    [DisplayName("英文名")]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置企业社会信用代码。
    /// </summary>
    [Form(Row = 2, Column = 2)]
    [DisplayName("社会信用代码")]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置企业地址。
    /// </summary>
    [Form(Row = 3, Column = 1)]
    [DisplayName("中文地址")]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置企业英文地址。
    /// </summary>
    [Form(Row = 4, Column = 1)]
    [DisplayName("英文地址")]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置企业联系人。
    /// </summary>
    [Form(Row = 5, Column = 1)]
    [DisplayName("联系人")]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置企业联系人电话。
    /// </summary>
    [Form(Row = 5, Column = 2)]
    [DisplayName("联系电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置企业备注。
    /// </summary>
    [Form(Row = 6, Column = 1, Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }
}