namespace Known;

public class Context
{
    private Language language;
    private string currentLanguage;

    public Context() { }

    internal Context(string cultureName)
    {
        language = new Language(cultureName);
    }

    internal SysModule Module { get; set; }
    public UserInfo CurrentUser { get; internal set; }

    public string CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            language = new Language(value);
            var culture = new CultureInfo(language.Name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    public Language Language
    {
        get
        {
            language ??= new Language(CurrentLanguage);
            return language;
        }
    }
}