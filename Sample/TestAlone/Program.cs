using Test.WinForm;

namespace TestAlone;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        AppHelper.Run();
    }
}