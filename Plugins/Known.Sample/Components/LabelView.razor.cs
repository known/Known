namespace Known.Sample.Components;

public partial class LabelView
{
    private object QrCode => new { Text = Code, Width = 200, Height = 200 };

    [Parameter] public string Title { get; set; }
    [Parameter] public string Code { get; set; }
    [Parameter] public Dictionary<string, object> Data { get; set; }
}