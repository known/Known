namespace Known.Razor;

public class KCaptcha : Field
{
    private const string Chars = "abcdefghijkmnpqrstuvwxyz2345678ABCDEFGHJKLMNPQRSTUVWXYZ";
    private string lastCode;
    private BaseRender<KCaptcha> render;

    public KCaptcha()
    {
        CanvasId = Utils.GetGuid();
        render = RenderFactory.Create<KCaptcha>();
        GenerateCode();
    }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    public string CanvasId { get; }
    public string Code { get; set; }

    public void GenerateCode()
    {
        var rnd = new Random();
        Code = "";
        for (int i = 0; i < 4; i++)
        {
            Code += Chars[rnd.Next(Chars.Length)];
        }
    }

    public bool Validate(out string message)
    {
        message = string.Empty;
        if (!Code.Equals(Value, StringComparison.OrdinalIgnoreCase))
        {
            message = "验证码不正确！";
            return false;
        }
        return true;
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Code != lastCode)
        {
            lastCode = Code;
            UI.Captcha(CanvasId, Code);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        render?.BuildTree(this, builder);
    }
}