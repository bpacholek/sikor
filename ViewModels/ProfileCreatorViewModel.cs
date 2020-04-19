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
using Sikor.Container;
using Sikor.Util.Ui;
using Sikor.Enum;

namespace Sikor.ViewModels
{
    public class ProfileCreatorViewModel : ReactiveViewServiceProvider
    {
        public void TestAndSave()
        {
            AppState.Loader.Show();

            _ = Task.Run(() => AppState.Jira.CreateProfile(ProfileName, Url, Username, Password)).ContinueWith(
                async r => {
                    if (r.IsFaulted)
                    {
                        _ = await MsgBox.Show("Input validation errors", r.Exception.InnerExceptions[0].Message, Icon.Error);
                    } else {
                        switch(r.Result)
                        {
                            case LoginState.SUCCESS:
                                await MsgBox.Show("Success", "New profile created successfully!", Icon.Success);
                            break;
                            case LoginState.INVALID_CREDENTIALS:
                                await MsgBox.Show("Invalid credentials", "Invalid credentials!", Icon.Forbidden);
                            break;
                            case LoginState.NETWORK_ERROR:
                                await MsgBox.Show("Connection problems", "Could not connect: please check the URL and your network connection.", Icon.Error);
                            break;
                        }
                        AppState.Loader.Hide();
                    }
            });
        }

        public string Url { get; set; }

        public string ProfileName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ProfileCreatorViewModel()
        {
            Url = "";
            Username = "";
            ProfileName = "";
            Password = "";
        }
    }
}
