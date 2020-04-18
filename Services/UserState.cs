using Sikor.ViewModels;
using Sikor.Container;
using Sikor.Model;

namespace Sikor.Services
{
    public class AppStateUpdater : ServiceProvider
    {
        #region references
        private MainWindowViewModel MainWindow;
        private SidebarViewModel Sidebar;
        private OperationsViewModel Operations;

        #endregion

        public Profile ActiveProfile;

        public override void Init()
        {
            MainWindow = ServiceContainer.GetServiceTyped<MainWindowViewModel>(typeof(MainWindowViewModel).GetType().ToString());
            Sidebar = ServiceContainer.GetServiceTyped<SidebarViewModel>(typeof(SidebarViewModel).GetType().ToString());
            Operations = ServiceContainer.GetServiceTyped<OperationsViewModel>(typeof(OperationsViewModel).GetType().ToString());
        }

        public void Login(Profile profile)
        {
            ActiveProfile = profile;
            sidebar.Init();
            currentTracking.init();
            mainWindow.LoginFormVisible = false;
        }
    }
}
