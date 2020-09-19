using Sikor.Services;
using System.Threading.Tasks;
using MessageBox.Avalonia.Enums;
using Sikor.Container;
using Sikor.Util.Ui;
using Sikor.Enum;
using Avalonia.Threading;
using sikor.Model;

namespace Sikor.ViewModels.Forms
{
    public class ProfileCreatorViewModel : ReactiveViewServiceProvider
    {
        public ProfileLoginDetails ProfileLoginDetails { get; private set; }

        public void TestAndSave()
        {
            AppState.Loader.Show();

            _ = Task.Run(() => AppState.Jira.CreateProfile(ProfileLoginDetails.ProfileName, ProfileLoginDetails.Url, ProfileLoginDetails.Username, ProfileLoginDetails.Password)).ContinueWith(
                async r =>
                {
                    if (r.IsFaulted)
                    {
                        await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Input validation errors", r.Exception.InnerExceptions[0].Message, Icon.Error));
                    }
                    else
                    {
                        switch (r.Result)
                        {
                            case LoginState.SUCCESS:
                                await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Success", "New profile created successfully!", Icon.Info));
                                AppState.ProfileSelector.ReloadProfiles(); //TODO make a meta-method
                                break;
                            case LoginState.INVALID_CREDENTIALS:
                                await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Invalid credentials", "Invalid credentials!", Icon.Forbidden));
                                break;
                            case LoginState.NETWORK_ERROR:
                                await Dispatcher.UIThread.InvokeAsync(async () => await MsgBox.Show("Connection problems", "Could not connect: please check the URL and your network connection.", Icon.Error));
                                break;
                        }
                    }
                    AppState.Loader.Hide();
                });
        }

        public ProfileCreatorViewModel()
        {
            ProfileLoginDetails = new ProfileLoginDetails();
        }
    }
}
