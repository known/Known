namespace Known.Sample.Models;

public class TestInfo
{
    public bool ShowField { get; set; }
    public string Field { get; set; }
    public string User { get; set; }
    public string Text { get; set; }
    public DateTime? Date { get; set; }

    public string[] Fields
    {
        get { return Field?.Split(','); }
        set { Field = string.Join(',', value); }
    }
}