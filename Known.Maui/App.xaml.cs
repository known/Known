namespace Known.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new MainPage()) { Title = "Known信息管理系统" };
        }
    }
}
