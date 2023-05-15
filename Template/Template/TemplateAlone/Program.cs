using Template.WinForm;

namespace TemplateAlone;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        AppHelper.Run();
    }
}