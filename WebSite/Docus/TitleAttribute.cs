namespace WebSite.Docus;

[AttributeUsage(AttributeTargets.Class)]
class TitleAttribute : Attribute
{
    public TitleAttribute(string title)
    {
        Title = title;
    }

    public string Title { get; }
}