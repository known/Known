namespace Known.WinForm.Pages;

public partial class Login
{
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