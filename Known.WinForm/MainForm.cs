using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;

namespace Known.WinForm;

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
        Text = "Known信息管理系统";
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
        services.AddBlazorWebViewDeveloperTools();
#endif
        services.AddApplication();
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