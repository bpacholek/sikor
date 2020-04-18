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
    public class ProfileCreatorViewModel : ReactiveObject
    {
        protected FullLoaderViewModel Loader;

        async public void TestAndSave()
        {
            Loader.Show();
            var JiraWrapper = new JiraWrapper();
            JiraWrapper.onValidationError += JiraWrapper_onValidationError;
            JiraWrapper.onInvalidCredentials += JiraWrapper_onInvalidCredentials;
            JiraWrapper.onNetworkIssue += JiraWrapper_onNetworkIssue;
            JiraWrapper.onSuccessfulLogin += JiraWrapper_onSuccessfulLogin;
            JiraWrapper.CreateProfile(ProfileName, Hostname, Username, Password);
        }

        public string Hostname { get; set; }

        public string ProfileName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }


        public ProfileCreatorViewModel()
        {

            Loader = ServicesContainer.GetServiceTyped<FullLoaderViewModel>("loader");
            Hostname = "";
            Username = "";
            ProfileName = "";
            Password = "";


    }

        async private void JiraWrapper_onValidationError(string message)
        {
            await MsgBox.Show("Input validation errors", message, Icon.Error);
            Loader.Hide();
        }

        async private void JiraWrapper_onSuccessfulLogin()
        {
            ServicesContainer.GetServiceTyped<ProfileSelectorViewModel>("ProfileSelectorViewModel").ReloadProfiles();
            await MsgBox.Show("Success", "Profile has been successfully added!", Icon.Success);

            Loader.Hide();

        }

        async private void JiraWrapper_onNetworkIssue()
        {
            await MsgBox.Show("Network error", "Could not connect to the provided URL. Please check your network connection...", Icon.Error);
            Loader.Hide();
        }

        async private void JiraWrapper_onInvalidCredentials()
        {
            await MsgBox.Show("Forbidden", "Invalid credentials", Icon.Forbidden);
            Loader.Hide();
        }
    }
}
