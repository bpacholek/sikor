using ReactiveUI;
using System.Reflection;
using Sikor.Container;
using System.Collections.Generic;

namespace Sikor.ViewModels
{
    public class MainWindowViewModel : ReactiveViewServiceProvider
    {
        protected bool loginPageVisible = true;

        protected bool trackingPageVisible = false;


        protected float logoOpacity = 0;

        public float LogoOpacity
        {
            get => logoOpacity;
            set => this.RaiseAndSetIfChanged(ref logoOpacity, value);
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

        public string Version
        {
            get
            {
                return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
        }
    }
}
