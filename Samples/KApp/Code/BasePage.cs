using Known.Razor;

namespace KApp;

public class BasePage : AppComponent
{
    //[Inject] protected DataService Service { get; set; }

    protected bool IsAdmin
    {
        get { return Client.CheckIsAdmin(CurrentUser); }
    }
}
