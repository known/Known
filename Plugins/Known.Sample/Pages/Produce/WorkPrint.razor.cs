using ZXing;
using ZXing.Common;

namespace Known.Sample.Pages.Produce;

public partial class WorkPrint
{
    private MarkupString svgWorkNo;
    private MarkupString svgCustGNo;

    [Parameter] public TbWork Work { get; set; }
    [Parameter] public string Printer { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            svgWorkNo = GenerateBarcode(Work?.WorkNo);
            svgCustGNo = GenerateBarcode(Work?.CustGNo);
            StateHasChanged();
        }
    }

    private static MarkupString GenerateBarcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new MarkupString();

        var writer = new BarcodeWriterSvg
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions { Height = 30, Margin = 0 }
        };
        var svg = writer.Write(value);
        return new MarkupString(svg.Content);
    }
}