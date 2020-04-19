using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Sentry;
using Sikor.Services;
using Atlassian.Jira;
using Sikor.Repository;
using Sikor.Model;
using System.IO;
using Sikor.Container;

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
                new JiraWrapper(),
                appState
            };

            foreach (Sikor.Container.IServiceProvider service in services){
                service.Register();
            }

            var Ui = BuildAvaloniaApp(); //adds view models to container
            var lifetime = new ClassicDesktopStyleApplicationLifetime();
            Ui.SetupWithLifetime(lifetime);
            Sikor.Container.ServiceContainer.Init();

            //Ui.StartWithClassicDesktopLifetime(args);
            appState.PostInit();

            foreach (Sikor.Container.IServiceProvider service in services){
                if (service.GetTypeString() == typeof(AppState).ToString()) {
                    continue; //skip appsstate
                }

                service.PostInit();
            }

            lifetime.Start(args);


/*
            using (SentrySdk.Init("https://79e453ed4dbd49a68acfd59bbe587d23@sentry.io/5166734"))
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            } */
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
