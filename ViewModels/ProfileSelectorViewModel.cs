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
using Sikor.Container;
using Sikor.Model;
using Sikor.Enum;

namespace Sikor.ViewModels
{
    public class ProfileSelectorViewModel : ReactiveViewServiceProvider
    {
        Settings Settings;

        public ObservableCollection<ListableItem> ProfileItems
        {
            get {
                var profileItems = new ObservableCollection<ListableItem>();
                var profilesList = AppState.Profiles.GetIdAndNamesList();

                var item = new ListableItem();
                item.Value = "- select profile -";
                item.Key = "-1";
                profileItems.Add(item);

                ListableItem selected = item;

                foreach (string id in profilesList.Keys)
                {
                    item = new ListableItem();
                    item.Value = profilesList[id];
                    item.Key = id;

                    if (id == Settings.LastSelectedProfile)
                    {
                        selected = item;
                    }

                    profileItems.Add(item);
                }

                selectedProfile = selected;
                return profileItems;
            }
        }

        protected ListableItem selectedProfile;
        public ListableItem SelectedProfile
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
            this.RaisePropertyChanged("ProfileItems");
        }

        public ProfileSelectorViewModel()
        {
            ReloadProfiles();
        }

        async public void DeleteSelected()
        {
            AppState.Loader.Show();
            if (SelectedProfile == null || SelectedProfile.Key == "-1")
            {
                await MsgBox.Show("Ups!", "No profile is selected!", Icon.Warning);
                AppState.Loader.Hide();
                return;
            }

            var result = await MsgBox.Show("Profile deletion", "Are you sure that you want to remove the selected project?", Icon.Warning, ButtonEnum.YesNo);
            if (result == ButtonResult.Yes)
            {
                //TODO move out of view model
                selectedProfile = null;
                Settings.LastSelectedProfile = null;
                Settings.Save();
                Profiles.Save();
                ReloadProfiles();
            }
            AppState.Loader.Hide();
        }

        async public void LoginAttempt()
        {
            AppState.Loader.Show();
            if (SelectedProfile == null || SelectedProfile.Key == "-1")
            {
                await MsgBox.Show("Ups!", "No profile is selected!", Icon.Warning);
                AppState.Loader.Hide();
                return;
            }

            var selectedProfile = Profiles.Get(SelectedProfile.Key);
            _ = Task.Run(() => AppState.Jira.Login(selectedProfile)).ContinueWith(
                async r => {
                    switch(r.Result)
                    {
                        case LoginState.SUCCESS:
                            AppState.Login(selectedProfile);
                        break;
                        case LoginState.INVALID_CREDENTIALS:
                            await MsgBox.Show("Invalid credentials", "Invalid credentials!", Icon.Forbidden);
                        break;
                        case LoginState.NETWORK_ERROR:
                            var result = await MsgBox.Show("Network connection", "Could not verify your login information due to network connection issues,\r\nyet if you are certain of your access details you may continue and try to operate on previously cached projects and issues.\r\nThis will allow you to track progress of your work offline and upload it when network connection is available.\r\nDo you want to continue?", Icon.Warning, ButtonEnum.YesNo);
                            if (result == ButtonResult.No)
                            {
                                //do nothing
                            } else
                            {
                                AppState.Login(selectedProfile);
                            }
                        break;
                    }
                    AppState.Loader.Hide();
            });
        }


    }
}
