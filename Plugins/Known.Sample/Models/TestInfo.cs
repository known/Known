namespace Known.Sample.Models;

public class TestInfo
{
    public bool ShowField { get; set; }

    [Form]
    public string Field { get; set; }

    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(UserPicker))]
    public string User { get; set; }

    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(OrgDropdownTree))]
    public string Organize { get; set; }

    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(UserDropdownTable))]
    public string UserName { get; set; }

    [Form]
    public DateTime? Date { get; set; }

    [Form(Type = nameof(FieldType.TextArea))]
    public string Text { get; set; }

    public string[] Fields
    {
        get { return Field?.Split(','); }
        set { Field = string.Join(',', value); }
    }
}