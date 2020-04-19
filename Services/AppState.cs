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

        public void SelectIssue(Issue issue)
        {
            ActiveProfile.SelectedIssue = issue;
            Operations.SelectIssue(issue);
        }

        public Profiles Profiles;

        public override void Init()
        {
            MainWindow = ServiceContainer.GetServiceTyped<MainWindowViewModel>(typeof(MainWindowViewModel).GetType().ToString());
            Sidebar = ServiceContainer.GetServiceTyped<SidebarViewModel>(typeof(SidebarViewModel).GetType().ToString());
            Operations = ServiceContainer.GetServiceTyped<OperationsViewModel>(typeof(OperationsViewModel).GetType().ToString());
            Loader = ServiceContainer.GetServiceTyped<FullLoaderViewModel>(typeof(FullLoaderViewModel).GetType().ToString());
            ProfileSelector = ServiceContainer.GetServiceTyped<ProfileSelectorViewModel>(typeof(ProfileSelectorViewModel).GetType().ToString());
            Jira = ServiceContainer.GetServiceTyped<JiraWrapper>(typeof(JiraWrapper).GetType().ToString());
            Profiles = ServiceContainer.GetServiceTyped<Profiles>(typeof(Profiles).GetType().ToString());
        }

        public void Login(Profile profile)
        {
            ActiveProfile = profile;
            //sidebar.Init();
            //currentTracking.init();
            MainWindow.LoginFormVisible = false;
        }

    }
}
