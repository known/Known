namespace Known.Extensions;

static class LanguageExtension
{
    internal static void Add(this List<SysLanguage> infos, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (infos.Exists(l => l.Chinese == name))
            return;

        infos.Add(new SysLanguage { AppId = Config.App.Id, Chinese = name });
    }
}