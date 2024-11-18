using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.Logging;

namespace Sample.WinForm;

public partial class MainForm : Form
{
    private readonly BlazorWebView blazorWebView;

    public MainForm()
    {
        CheckForIllegalCrossThreadCalls = false;
        InitializeComponent();

        AppSetting.Load();
        blazorWebView = new BlazorWebView();
        blazorWebView.Dock = DockStyle.Fill;
        blazorWebView.BlazorWebViewInitialized = new EventHandler<BlazorWebViewInitializedEventArgs>(WebViewInitialized);
        Controls.Add(blazorWebView);
        AddBlazorWebView();

        WindowState = FormWindowState.Maximized;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Text = $"{AppConfig.Branch}-{AppConfig.SubTitle}";
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        var result = Dialog.Confirm("确定退出系统？");
        if (result == DialogResult.Cancel)
            e.Cancel = true;
        else
            OnClose();
    }

    private void WebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
    {
        e.WebView.ZoomFactor = AppSetting.ZoomFactor;
    }

    private void AddBlazorWebView()
    {
        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
#if DEBUG
        Config.IsDevelopment = true;
        services.AddBlazorWebViewDeveloperTools();
#endif
        var isClient = false;
        if (isClient)
        {
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000") });
            services.AddSampleClient();
        }
        else
        {
            services.AddSampleCore();
            services.AddKnownCore(info =>
            {
                //info.ProductId = "Test";
                //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
                info.Type = AppType.Desktop;
                info.WebRoot = Application.StartupPath;
                info.ContentRoot = Application.StartupPath;
                info.Assembly = typeof(Program).Assembly;
            });
            services.AddKnownData(option =>
            {
                option.AddProvider<Microsoft.Data.Sqlite.SqliteFactory>("Default", DatabaseType.SQLite, "Data Source=..\\Sample.db");
                //option.AddProvider<System.Data.OleDb.OleDbFactory>("Default", DatabaseType.Access, "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password={password}");
                //option.AddProvider<System.Data.SqlClient.SqlClientFactory>("Default", DatabaseType.SqlServer, "Server=(localdb)\\MSSQLLocalDB;Database=Sample;Trusted_Connection=True");
                //option.AddProvider<MySqlConnector.MySqlConnectorFactory>("Default", DatabaseType.MySql, "Data Source=localhost;port=3306;Initial Catalog=Sample;user id={userId};password={password};Charset=utf8;SslMode=none;AllowZeroDateTime=True;");
                //option.AddProvider<Npgsql.NpgsqlFactory>("Default", DatabaseType.PgSql, "Data Source=localhost;Initial Catalog=Sample;User Id={userId};Password={password};");
                //option.AddProvider<Dm.DmClientFactory>("Default", DatabaseType.DM, "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;");
                //option.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
            });
            services.AddKnownCells();
            services.AddKnownWin();
        }

        blazorWebView.HostPage = "wwwroot\\index.html";
        blazorWebView.Services = services.BuildServiceProvider();
        blazorWebView.RootComponents.Add<App>("#app");
        Config.OnExit = OnClose;
        Config.ServiceProvider = blazorWebView.Services;
    }

    private void OnClose()
    {
        AppSetting.ZoomFactor = blazorWebView.WebView.ZoomFactor;
        AppSetting.Save();
        Environment.Exit(0);
    }
}