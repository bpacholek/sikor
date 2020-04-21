using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Sikor.Services;
using System.IO;

namespace Sikor
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--run-test")
            {
                StreamWriter testWriter = new StreamWriter("test.log");
                testWriter.WriteLine("it-works");
                testWriter.Close();
                return;
            }

            var appState = new AppState();

            Sikor.Container.IServiceProvider[] services = {
                new Storage(),
                new Logger(),
                new JiraWrapper(),
                appState
            };

            foreach (Sikor.Container.IServiceProvider service in services)
            {
                service.Register();
            }

            var Ui = BuildAvaloniaApp(); //adds view models to container
            var lifetime = new ClassicDesktopStyleApplicationLifetime();
            Ui.SetupWithLifetime(lifetime);
            Sikor.Container.ServiceContainer.Init();

            //Ui.StartWithClassicDesktopLifetime(args);
            appState.PostInit();

            foreach (Sikor.Container.IServiceProvider service in services)
            {
                if (service.GetTypeString() == typeof(AppState).ToString())
                {
                    continue; //skip appsstate
                }

                service.PostInit();
            }
            lifetime.Start(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .LogToDebug()
                .UsePlatformDetect()
                .With(new X11PlatformOptions { UseGpu = false })
                .With(new AvaloniaNativePlatformOptions { UseGpu = false })
                .With(new MacOSPlatformOptions { ShowInDock = false })
                .With(new Win32PlatformOptions { UseDeferredRendering = false })
                .UseReactiveUI();
    }
}
