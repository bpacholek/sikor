using ReactiveUI;
using System.Reflection;
using Sikor.Container;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sikor.ViewModels
{
    public class MainWindowViewModel : ReactiveViewServiceProvider
    {
        protected bool loginPageVisible = true;

        protected bool trackingPageVisible = false;

        protected string latestVersion = "Loading...";

        protected string updateUrl = null;

        protected float logoOpacity = 0;

        protected bool updateVisible = false;

        public float LogoOpacity
        {
            get => logoOpacity;
            set => this.RaiseAndSetIfChanged(ref logoOpacity, value);
        }

        public bool UpdateVisible
        {
            get => updateVisible;
            set => this.RaiseAndSetIfChanged(ref updateVisible, value);
        }
        public bool LoginPageVisible
        {
            get => loginPageVisible;
            set => this.RaiseAndSetIfChanged(ref loginPageVisible, value);
        }

        public bool TrackingPageVisible
        {
            get => trackingPageVisible;
            set => this.RaiseAndSetIfChanged(ref trackingPageVisible, value);
        }

        public string LatestVersion
        {
            get => latestVersion;
            set => this.RaiseAndSetIfChanged(ref latestVersion, value);
        }

        public string UpdateUrl
        {
            get => updateUrl;
            set => this.RaiseAndSetIfChanged(ref updateUrl, value);
        }

        public string Version
        {
            get
            {
                return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
        }

        private void OpenUpdate()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(updateUrl)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

    }
}
