using Sikor.ViewModels;
using Sikor.Container;
using Sikor.Model;
using Sikor.Repository;

namespace Sikor.Services
{
    public class AppState : ServiceProvider
    {
        #region references
        public MainWindowViewModel MainWindow { get; private set; }
        public SidebarViewModel Sidebar { get; private set; }
        public OperationsViewModel Operations { get; private set; }

        public ProfileSelectorViewModel ProfileSelector { get; private set; }
        public FullLoaderViewModel Loader { get; private set; }

        public JiraWrapper Jira { get; private set; }
        #endregion

        public Profile ActiveProfile;

        public Settings Settings;

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
            Operations = ServiceContainer.GetServiceTyped<OperationsViewModel>(typeof(OperationsViewModel).ToString());

            Loader = ServiceContainer.GetServiceTyped<FullLoaderViewModel>(typeof(FullLoaderViewModel).ToString());
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
            }
            else
            {
                Profiles = new Profiles();
            }

            Profiles.Init();

            ProfileSelector.ReloadProfiles();
        }

        public void Login(Profile profile)
        {
            ActiveProfile = profile;
            Sidebar.PostInit();
            Operations.PostInit();
            MainWindow.LoginFormVisible = false;
        }

    }
}
