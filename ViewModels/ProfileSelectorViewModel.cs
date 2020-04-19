using ReactiveUI;
using System.Collections.ObjectModel;
using Sikor.Services;
using System.Threading.Tasks;
using Sikor.Model;
using MessageBox.Avalonia.Enums;
using Sikor.Util.Ui;
using Sikor.Container;
using Sikor.Enum;
using Avalonia.Threading;

namespace Sikor.ViewModels
{
    public class ProfileSelectorViewModel : ReactiveViewServiceProvider
    {

        public ObservableCollection<ListableItem> ProfileItems
        {
            get
            {
                if (AppState.Profiles == null)
                {
                    return null;
                }

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

                    if (id == AppState.Settings.LastSelectedProfile)
                    {
                        selected = item;
                    }

                    profileItems.Add(item);
                }

                SelectedProfile = selected;
                return profileItems;
            }

        }

        protected ListableItem selectedProfile;
        public ListableItem SelectedProfile
        {
            get => selectedProfile;
            set => this.RaiseAndSetIfChanged(ref selectedProfile, value);
        }

        public void ReloadProfiles()
        {
            this.RaisePropertyChanged("ProfileItems");
            this.RaisePropertyChanged("SelectedProfile");
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
                AppState.Profiles.DeleteByKey(selectedProfile.Key);
                //TODO move out of view model
                SelectedProfile = null;
                AppState.Settings.LastSelectedProfile = null;
                AppState.Settings.Save();
                AppState.Profiles.Save();
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

            var selectedProfile = AppState.Profiles.Get(SelectedProfile.Key);
            _ = Task.Run(() => AppState.Jira.Login(selectedProfile)).ContinueWith(
                async r =>
                {
                    switch (r.Result)
                    {
                        case LoginState.SUCCESS:
                            AppState.Login(selectedProfile);
                            break;
                        case LoginState.INVALID_CREDENTIALS:
                            await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Invalid credentials", "Invalid credentials!", Icon.Forbidden));
                            break;
                        case LoginState.NETWORK_ERROR:
                            var result = await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Network connection", "Could not verify your login information due to network connection issues,\r\nyet if you are certain of your access details you may continue and try to operate on previously cached projects and issues.\r\nThis will allow you to track progress of your work offline and upload it when network connection is available.\r\nDo you want to continue?", Icon.Warning, ButtonEnum.YesNo));
                            if (result == ButtonResult.No)
                            {
                                //do nothing
                            }
                            else
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
