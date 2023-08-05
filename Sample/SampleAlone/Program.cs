using Sample.WinForm;

namespace SampleAlone;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        AppAlone.Run();
    }
}