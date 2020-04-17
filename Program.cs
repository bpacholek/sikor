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

            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var storage = new Storage(appPath);
            ServicesContainer.RegisterService("storage", storage);

            if (storage.Has("profiles"))
            {
                ServicesContainer.RegisterService("profiles", storage.Get<Profiles>("profiles"));
            }
            else
            {
                ServicesContainer.RegisterService("profiles", new Profiles());
            }

            if (storage.Has("settings"))
            {
                ServicesContainer.RegisterService("settings", storage.Get<Settings>("settings"));
            }
            else
            {
                ServicesContainer.RegisterService("settings", new Settings());
            }

            ServicesContainer.RegisterService("jira_wrapper", new JiraWrapper());


            using (SentrySdk.Init("https://79e453ed4dbd49a68acfd59bbe587d23@sentry.io/5166734"))
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
