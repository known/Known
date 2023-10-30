using System.ComponentModel;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Studio
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            AppSetting.Load();
            WindowState = FormWindowState.Maximized;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = "Known Studio 1.0";
            blazorWebView.BlazorWebViewInitialized = new EventHandler<BlazorWebViewInitializedEventArgs>(WebViewInitialized);
            AddBlazorWebView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
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

        private void WebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ZoomFactor = AppSetting.ZoomFactor;
        }

        private void AddBlazorWebView()
        {
            var services = new ServiceCollection();
            services.AddKnown();
            services.AddWindowsFormsBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = services.BuildServiceProvider();
            blazorWebView.RootComponents.Add<App>("#app");
        }
    }
}