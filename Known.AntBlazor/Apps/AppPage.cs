namespace Known.AntBlazor.Apps;

public class AppPage : BaseComponent
{
    [CascadingParameter] protected AppIndex App { get; set; }
}