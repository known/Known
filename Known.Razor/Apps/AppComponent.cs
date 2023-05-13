namespace Known.Razor.Apps;

public abstract class AppComponent : BaseComponent
{
    [Inject] protected NavigationManager Navigation { get; set; }
}