using Sikor.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Services
{
    public class UserState : IService
    {
        public Model.Profile UserProfile; 
        public UserState()
        {
            ServicesContainer.RegisterService("state", this);
        }

        public void Login(Model.Profile userProfile)
        {
            UserProfile = userProfile;
            var mainWindow = ServicesContainer.GetServiceTyped<MainWindowViewModel>("MainWindow");
            var sidebar = ServicesContainer.GetServiceTyped<Sidebar>("sidebar");
            sidebar.Init();
            var currentTracking = ServicesContainer.GetServiceTyped<CurrentTrackingViewModel>("current_tracking");
            currentTracking.init();
            mainWindow.LoginFormVisible = false;
        }
    }
}
