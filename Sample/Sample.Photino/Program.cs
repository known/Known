using Known;
using Photino.Blazor;

namespace Sample.Photino;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var builder = PhotinoBlazorAppBuilder.CreateDefault(args);
        builder.Services.AddKnown();

        // register root component and selector
        builder.RootComponents.Add<App>("app");

        var app = builder.Build();

        // customize window
        app.MainWindow.SetIconFile("favicon.ico")
                      .SetTitle("Known信息管理系统")
                      .Center()
                      .SetMaximized(true);

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }
}