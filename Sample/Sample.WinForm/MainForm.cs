using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;

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
        services.AddApplication(info =>
        {
            info.Type = AppType.Desktop;
            info.WebRoot = Application.StartupPath;
            info.ContentRoot = Application.StartupPath;
            info.Assembly = typeof(Program).Assembly;
            //数据库连接
            info.Connections = [new ConnectionInfo
            {
                Name = "Default",
                DatabaseType = DatabaseType.SQLite,
                ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
                ConnectionString = "Data Source=..\\Sample.db"
                //DatabaseType = DatabaseType.Access,
                //ProviderType = typeof(System.Data.OleDb.OleDbFactory),
                //ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password={password}";
                //DatabaseType = DatabaseType.SqlServer,
                //ProviderType = typeof(System.Data.SqlClient.SqlClientFactory),
                //ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Sample;Trusted_Connection=True";
                //DatabaseType = DatabaseType.MySql,
                //ProviderType = typeof(MySqlConnector.MySqlConnectorFactory),
                //ConnectionString = "Data Source=localhost;port=3306;Initial Catalog=Sample;user id={userId};password={password};Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
                //DatabaseType = DatabaseType.PgSql,
                //ProviderType = typeof(Npgsql.NpgsqlFactory),
                //ConnectionString = "Data Source=localhost;Initial Catalog=Sample;User Id={userId};Password={password};";
            }];
        });
        blazorWebView.HostPage = "wwwroot\\index.html";
        blazorWebView.Services = services.BuildServiceProvider();
        blazorWebView.Services.UseApplication();
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