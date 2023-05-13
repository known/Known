namespace Test.WinForm;

partial class MainForm : Form
{
    public MainForm()
    {
        CheckForIllegalCrossThreadCalls = false;
        InitializeComponent();
        AutoScaleMode = AutoScaleMode.Dpi;
        WindowState = FormWindowState.Maximized;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Text = Config.AppName;

        blazorWebView.BlazorWebViewInitialized = new EventHandler<BlazorWebViewInitializedEventArgs>(WebViewInitialized);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        AddBlazorWebView();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        var result = Dialog.Confirm("确定退出系统？");
        if (result == DialogResult.Cancel)
        {
            e.Cancel = true;
        }
        else
        {
            AppSetting.ZoomFactor = blazorWebView.WebView.ZoomFactor;
            AppSetting.Save();
            Environment.Exit(0);
        }
    }

    private void AddBlazorWebView()
    {
        var services = new ServiceCollection();
        services.AddAlone();
        services.AddWindowsFormsBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
        blazorWebView.HostPage = "wwwroot\\index.html";
        blazorWebView.Services = services.BuildServiceProvider();
        blazorWebView.RootComponents.Add<App>("#app");
        blazorWebView.Visible = true;
    }

    private void WebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
    {
        e.WebView.ZoomFactor = AppSetting.ZoomFactor;
    }
}