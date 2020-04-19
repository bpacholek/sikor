using ReactiveUI;
using System.Reflection;
using Sikor.Container;

namespace Sikor.ViewModels
{
    public class MainWindowViewModel : ReactiveViewServiceProvider
    {
        protected bool loginFormVisible = true;
        public bool LoginFormVisible
        {
            get => loginFormVisible;
            set => this.RaiseAndSetIfChanged(ref loginFormVisible, value);
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
