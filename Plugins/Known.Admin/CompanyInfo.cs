namespace Known;

/// <summary>
/// 企业信息类。
/// </summary>
public class CompanyInfo
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [Form(Row = 1, Column = 1, ReadOnly = true), Required]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [Form(Row = 1, Column = 2), Required]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置企业英文名。
    /// </summary>
    [Form(Row = 2, Column = 1)]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置企业社会信用代码。
    /// </summary>
    [Form(Row = 2, Column = 2)]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置企业地址。
    /// </summary>
    [Form(Row = 3, Column = 1)]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置企业英文地址。
    /// </summary>
    [Form(Row = 4, Column = 1)]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置企业联系人。
    /// </summary>
    [Form(Row = 5, Column = 1)]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置企业联系人电话。
    /// </summary>
    [Form(Row = 5, Column = 2)]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置企业备注。
    /// </summary>
    [Form(Row = 6, Column = 1, Type = nameof(FieldType.TextArea))]
    [MaxLength(500)]
    public string Note { get; set; }
}