namespace Known.Wasm.Pages;

public partial class Login
{
    private object QrCode => new { Text = Config.HostUrl, Width = 200, Height = 200 };

    protected override Task OnInitAsync()
    {
        Model.UserName = "Admin";
        Model.Password = "1";
        return base.OnInitAsync();
    }

    private Task<Result> OnSendSMS(string phone)
    {
        return Result.SuccessAsync("发送成功！");
    }
}