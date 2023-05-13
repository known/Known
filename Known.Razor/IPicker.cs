namespace Known.Razor;

public interface IPicker
{
    string Title { get; }
    Size Size { get; }
    Action<object> OnOK { get; set; }
    void BuildPick(RenderTreeBuilder builder);
}