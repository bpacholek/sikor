using Sikor.ViewModels;
using Sikor.ViewModels.Utils;
using Sikor.ViewModels.Forms;

using Sikor.Container;
using Sikor.Model;
using Sikor.Repository;
using System;

namespace Sikor.Services
{
    public class AppState : ServiceProvider
    {
        #region references
        public MainWindowViewModel MainWindow { get; private set; }
        public SidebarViewModel Sidebar { get; private set; }
        public OperationsViewModel Operations { get; private set; }

        public ProfileSelectorViewModel ProfileSelector { get; private set; }
        public OnTopLoaderViewModel Loader { get; private set; }

        public JiraWrapper Jira { get; private set; }
        #endregion

        public Profile ActiveProfile;

        public Settings Settings;

        public Logger Logger;
        public Profiles Profiles;

        public void SelectIssue(Issue issue)
        {
            ActiveProfile.SelectedIssue = issue;
            Operations.UpdateSelectionProperties();
        }


        public override void PostInit()
        {
            MainWindow = ServiceContainer.GetServiceTyped<MainWindowViewModel>(typeof(MainWindowViewModel).ToString());
            Sidebar = ServiceContainer.GetServiceTyped<SidebarViewModel>(typeof(SidebarViewModel).ToString());
            Logger = ServiceContainer.GetServiceTyped<Logger>(typeof(Logger).ToString());
            Operations = ServiceContainer.GetServiceTyped<OperationsViewModel>(typeof(OperationsViewModel).ToString());
            Loader = ServiceContainer.GetServiceTyped<OnTopLoaderViewModel>(typeof(OnTopLoaderViewModel).ToString());
            ProfileSelector = ServiceContainer.GetServiceTyped<ProfileSelectorViewModel>(typeof(ProfileSelectorViewModel).ToString());
            Jira = ServiceContainer.GetServiceTyped<JiraWrapper>(typeof(JiraWrapper).ToString());
            var storage = ServiceContainer.GetServiceTyped<Storage>(typeof(Storage).ToString());

            var settingsName = typeof(Settings).ToString();
            if (storage.Has(settingsName))
            {
                Settings = storage.Get<Settings>(settingsName);
            }
            else
            {
                Settings = new Settings();
            }

            Settings.Init();

            var profilesName = typeof(Profiles).ToString();
            if (storage.Has(profilesName))
            {
                Profiles = storage.Get<Profiles>(profilesName);

                if (Profiles == default(Profiles) || Profiles == null)
                {
                    Util.Ui.MsgBox.Show("Error", "Corrupted profiles file. This is a known bug and will be solved in 0.3; until that the only solution is to remove your profiles files and start again. If you continue then application will overwrite the profiles file for you.", MessageBox.Avalonia.Enums.Icon.Error, MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                    Profiles = new Profiles();
                }
            }
            else
            {
                Profiles = new Profiles();
            }

            Profiles.Init();

            ProfileSelector.ReloadProfiles();
            MainWindow.LogoOpacity = 1;
        }

        public void Login(Profile profile)
        {
            ActiveProfile = profile;
            Sidebar.PostInit();
            Operations.PostInit();
            MainWindow.LoginPageVisible = false;
            MainWindow.TrackingPageVisible  = true;
        }

    }
}
