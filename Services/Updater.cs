using Sikor.ViewModels;
using Sikor.ViewModels.Utils;
using Sikor.ViewModels.Forms;

using Sikor.Container;
using Sikor.Model;
using Sikor.Repository;
using System;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Avalonia.Threading;
using System.Threading.Tasks;
namespace Sikor.Services
{
    public class Updater : ServiceProvider
    {
        public MainWindowViewModel MainWindow { get; private set; }

        public override void PostInit()
        {
            MainWindow = ServiceContainer.GetServiceTyped<MainWindowViewModel>(typeof(MainWindowViewModel).ToString());
            Task.Run(() => LoadLatestVersion());
        }

        private async void LoadLatestVersion()
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://api.github.com/repos/ideaconnect/sikor/releases/latest");
            request.UserAgent = "Sikor Jira-client by IDCT";
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream())) {
                string contents = reader.ReadToEnd();
                var release = JsonConvert.DeserializeObject<GithubRelease>(contents);
                await Dispatcher.UIThread.InvokeAsync(() => UpdateText(release));
            }
            //Task.Run(() => );
        }

        private void UpdateText(GithubRelease githubRelease)
        {
            MainWindow.LatestVersion = githubRelease.TagName;
            MainWindow.UpdateUrl = githubRelease.HtmlUrl;
            MainWindow.UpdateVisible = MainWindow.LatestVersion != MainWindow.Version;
        }

    }
}
