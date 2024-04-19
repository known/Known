namespace Known.Demo.Models;

class CompanyInfo
{
    [Form(Row = 1, Column = 1, ReadOnly = true), Required]
    public string Code { get; set; }

    [Form(Row = 1, Column = 2), Required]
    public string Name { get; set; }

    [Form(Row = 2, Column = 1)]
    public string NameEn { get; set; }

    [Form(Row = 2, Column = 2)]
    public string SccNo { get; set; }

    [Form(Row = 3, Column = 1)]
    public string Address { get; set; }

    [Form(Row = 4, Column = 1)]
    public string AddressEn { get; set; }

    [Form(Row = 5, Column = 1)]
    public string Contact { get; set; }

    [Form(Row = 5, Column = 2)]
    public string Phone { get; set; }

    [Form(Row = 6, Column = 1, Type = "TextArea")]
    [MaxLength(500)]
    public string Note { get; set; }
}