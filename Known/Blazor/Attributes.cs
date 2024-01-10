namespace Known.Blazor;

[AttributeUsage(AttributeTargets.Class)]
public class PageAttribute : Attribute
{
    public bool NoBackground { get; set; }
}