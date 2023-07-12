using System.ComponentModel;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Test
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            AppSetting.Load();
            WindowState = FormWindowState.Maximized;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Config.AppName;
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
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppHelper.Url) });
            services.AddWindowsFormsBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = services.BuildServiceProvider();
            blazorWebView.RootComponents.Add<App>("#app");
            blazorWebView.Visible = true;
        }
    }
}