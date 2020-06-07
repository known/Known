using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace KRunner
{
    class WinService : ServiceBase
    {
        private IContainer components = null;
        private readonly Action start;
        private readonly Action stop;

        public WinService(Action start,Action stop)
        {
            this.start = start;
            this.stop = stop;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            start.Invoke();
        }

        protected override void OnStop()
        {
            stop.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
        }
    }

    [RunInstaller(true)]
    public class WinServiceInstaller : Installer
    {
        private IContainer components = null;
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;
        public static string ServiceName;
        public static string ServiceDesc;

        public WinServiceInstaller()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            processInstaller.Password = null;
            processInstaller.Username = null;

            serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.Description = ServiceDesc;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.AfterInstall += new InstallEventHandler(ServiceInstallerAfterInstall);

            Installers.AddRange(new Installer[] { processInstaller, serviceInstaller });
        }

        private void ServiceInstallerAfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceInstaller.ServiceName, true);
                object value = regKey.GetValue("ImagePath");
                if (value != null)
                {
                    regKey.SetValue("ImagePath", string.Format("{0} -s", Process.GetCurrentProcess().MainModule.FileName));
                    regKey.Flush();
                }
                regKey.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}
