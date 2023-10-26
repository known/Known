using Known.Demo.Services;

namespace Known.Demo;

public class ClientFactory
{
    public ClientFactory(UserInfo user)
    {
        Home = new HomeService { CurrentUser = user };
        Apply = new ApplyService { CurrentUser = user };
    }

    public HomeService Home { get; }
    public ApplyService Apply { get; }
}