using Known;
using Microsoft.Extensions.Logging;

namespace Sample.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddKnown(info =>
            {
                info.Id = "AppId";
                info.Name = "AppName";
                info.Assembly = typeof(MauiProgram).Assembly;
            });
            builder.Services.AddKnownClient(option => option.BaseAddress = "http://localhost:5000");
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
