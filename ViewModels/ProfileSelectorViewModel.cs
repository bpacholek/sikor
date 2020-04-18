using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Atlassian.Jira;
using Sikor.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sikor.Model;
using Sikor.Repository;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia;
using Sikor.Util.Ui;
namespace Sikor.ViewModels
{
    public class ProfileSelectorViewModel : ReactiveObject, IService
    {
        Settings Settings;

        protected ObservableCollection<ListItem> profileItems;
        public ObservableCollection<ListItem> ProfileItems
        {
            get => profileItems;
            set => this.RaiseAndSetIfChanged(ref profileItems, value);
        }

        protected ListItem selectedProfile;
        public ListItem SelectedProfile
        {
            get => selectedProfile;
            set {
                if (value != null)
                {
                    this.RaiseAndSetIfChanged(ref selectedProfile, value);
                }
            }
        }

        Profiles Profiles;

        public void ReloadProfiles()
        {
            var profileItems = new ObservableCollection<Model.ListItem>();
            var profilesList = Profiles.GetIdList();

            var item = new ListItem();
            item.Name = "- select profile -";
            item.Value = "-1";
            item.Key = "-1";
            profileItems.Add(item);

            ListItem selected = item;

            foreach (string id in profilesList.Keys)
            {
                item = new ListItem();
                item.Name = profilesList[id];
                item.Value = profilesList[id];
                item.Key = id;

                if (id == Settings.LastSelectedProfile)
                {
                    selected = item;
                }

                profileItems.Add(item);
            }

            selectedProfile = selected;
            ProfileItems = profileItems;

        }

        public ProfileSelectorViewModel()
        {
            Loader = ServicesContainer.GetServiceTyped<FullLoaderViewModel>("loader");

            Settings = ServicesContainer.GetServiceTyped<Settings>("settings");
            ServicesContainer.RegisterService("ProfileSelectorViewModel", this);
            Profiles = ServicesContainer.GetServiceTyped<Profiles>("profiles");
            ReloadProfiles();
        }


        async public void DeleteSelected()
        {
            var Loader = ServicesContainer.GetServiceTyped<FullLoaderViewModel>("loader");
            Loader.Show();
            if (SelectedProfile == null || SelectedProfile.Key == "-1")
            {
                await MsgBox.Show("Ups!", "No profile is selected!", Icon.Warning);
                Loader.Hide();
                return;
            }

            var question = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = MessageBox.Avalonia.Enums.ButtonEnum.YesNo,
                ContentTitle = "Confirmation",
                ContentMessage = "Are you sure that you want to remove the selected project?",
                Icon = MessageBox.Avalonia.Enums.Icon.Info,
                Style = MessageBox.Avalonia.Enums.Style.None
            });

            var result = await question.Show();
            if (result == ButtonResult.Yes)
            {
                Profiles.DeleteByKey(SelectedProfile.Key);
                selectedProfile = null;
                Settings.LastSelectedProfile = null;
                Settings.Save();
                Profiles.Save();
                ReloadProfiles();
            }
            Loader.Hide();

        }

        FullLoaderViewModel Loader;
        async public void LoginAttempt()
        {
            Loader.Show();
            if (SelectedProfile == null || SelectedProfile.Key == "-1")
            {
                await MsgBox.Show("Ups!", "No profile is selected!", Icon.Warning);
                Loader.Hide();
                return;
            }
            var selectedProfile = Profiles.Get(SelectedProfile.Key);

            var JiraWrapper = new JiraWrapper();
            JiraWrapper.onValidationError += JiraWrapper_onValidationError;
            JiraWrapper.onInvalidCredentials += JiraWrapper_onInvalidCredentials;
            JiraWrapper.onNetworkIssue += JiraWrapper_onNetworkIssue;
            JiraWrapper.onSuccessfulLogin += JiraWrapper_onSuccessfulLogin;
            JiraWrapper.AtteptLogin(selectedProfile);

        }

        async private void JiraWrapper_onValidationError(string message)
        {
            await MsgBox.Show("Input validation errors", message, Icon.Error);
            Loader.Hide();
        }

        async private void JiraWrapper_onSuccessfulLogin()
        {
            Loader.Hide();
            Login();
        }

        private void Login()
        {
            var stateController = new UserState();
            stateController.Login(Profiles.Get(SelectedProfile.Key));
        }

        async private void JiraWrapper_onNetworkIssue()
        {
            var result = await MsgBox.Show("Network connection", "Could not verify your login information due to network connection issues,\r\nyet if you are certain of your access details you may continue and try to operate on previously cached projects and issues.\r\nThis will allow you to track progress of your work offline and upload it when network connection is available.\r\nDo you want to continue?", Icon.Warning, ButtonEnum.YesNo);
            Loader.Hide();
            if (result == ButtonResult.No)
            {
                //do nothing
            } else
            {
                Login();
            }
        }

        async private void JiraWrapper_onInvalidCredentials()
        {
            await MsgBox.Show("Forbidden", "Invalid credentials", Icon.Forbidden);
            Loader.Hide();
        }
    }
}
