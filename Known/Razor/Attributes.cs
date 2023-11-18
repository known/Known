namespace Known.Razor;

[AttributeUsage(AttributeTargets.Class)]
public class PageAttribute : Attribute
{
    public bool NoBreadcrumb { get; set; }
}