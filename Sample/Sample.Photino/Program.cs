using Photino.Blazor;

namespace Sample.Photino;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var builder = PhotinoBlazorAppBuilder.CreateDefault(args);
        builder.Services.AddApplication();
        builder.RootComponents.Add<App>("app");

        var app = builder.Build();
        app.MainWindow.SetIconFile("favicon.ico")
                      .SetTitle("Known信息管理系统")
                      .Center()
                      .SetMaximized(true);

        // Hide the console window
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    private const int SW_HIDE = 0;
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}