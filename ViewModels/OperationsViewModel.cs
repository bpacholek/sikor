using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Sikor.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sikor.Model;
using Sikor.Repository;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Sikor.Util.Ui;
using System.Timers;
using System.Diagnostics;
using Sikor.Container;

namespace Sikor.ViewModels
{

    public class OperationsViewModel : ReactiveViewServiceProvider
    {
        public string TrackingTime { get; set; }
        Timer TimeUpdater;
        public bool IsSelected =>  (AppState.ActiveProfile != null ? AppState.ActiveProfile.SelectedIssue != null : false);
        public bool IsTracking => (AppState.ActiveProfile != null ? AppState.ActiveProfile.IsTracking : false);

        public Tracking TrackedIssue => (AppState.ActiveProfile != null ? AppState.ActiveProfile.CurrentTracking : null);
        public bool HasFailedStatuses => (AppState.ActiveProfile != null ? AppState.ActiveProfile.FailedStatusUpdates.Count > 0 : false);
        public bool HasFailedWorklogs => (AppState.ActiveProfile != null ? AppState.ActiveProfile.FailedWorklogs.Count > 0 : false);
        public Issue SelectedIssue => (AppState.ActiveProfile != null ? AppState.ActiveProfile.SelectedIssue : null);
        public ListableItem SelectedNewStatus {get;set;}

        public FailedStatusUpdate SelectedFailedStatus { get; set; }

        public Tracking SelectedFailedWorklog { get; set; }


        public int SelectedFailedWorklogIndex { get; set; }

        public int SelectedFailedIndex { get; set; }
        private void TimeUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsTracking)
            {
                var diff = DateTime.Now - AppState.ActiveProfile.CurrentTracking.Created;
                TrackingTime = Math.Floor(diff.TotalHours).ToString().PadLeft(2, '0') + ":" + diff.Minutes.ToString().PadLeft(2,'0') + ":" + diff.Seconds.ToString().PadLeft(2,'0');
                this.RaisePropertyChanged("TrackingTime");
            }
        }

        async public void StoreTracking(bool saveOnFailure = false)
        {
            AppState.Loader.Show();
            if ((DateTime.Now - AppState.ActiveProfile.CurrentTracking.Created).TotalMinutes < 1)
            {
                var result = await MsgBox.Show("Warning", "Sadly, Jira cannot track anything under a minute. Do you wish to save this tracking as 1 minute [Yes] or continue tracking [Cancel]?", Icon.Warning, ButtonEnum.OkCancel);
                if (result == ButtonResult.Cancel)
                {
                    AppState.Loader.Hide();
                    return;
                }
            }

            _ = Task.Run(() => AppState.Jira.StoreWorklog(AppState.ActiveProfile.CurrentTracking, saveOnFailure).ContinueWith(
                async r => {
                    if (r.Result == false) {
                        var result = await MsgBox.Show("Failed", "Failed to store the worklog. Do you wish to save it for later if next attempt fails [Yes], just retry [No] or abandon [Abort]?", Icon.Error, ButtonEnum.YesNoAbort);
                        if (result != ButtonResult.Abort) {
                            StoreTracking(result == ButtonResult.Yes);
                        }
                        return;
                    } else {
                        await MsgBox.Show("Success", "Worklog successfully saved", Icon.Success, ButtonEnum.Ok);
                        AppState.ActiveProfile.CurrentTracking = null;
                    }
            }));
        }


        public ObservableCollection<FailedStatusUpdate> FailedStatuses
        {
            get
            {
                if (HasFailedStatuses)
                {
                    var collection = new ObservableCollection<FailedStatusUpdate>();
                    foreach(FailedStatusUpdate status in AppState.ActiveProfile.FailedStatusUpdates)
                    {
                        collection.Add(status);
                    }
                    return collection;
                }

                return null;
            }
        }

        public ObservableCollection<Tracking> FailedWorklogs
        {
            get
            {
                if (HasFailedWorklogs)
                {
                    var collection = new ObservableCollection<Tracking>();
                    foreach(Tracking status in AppState.ActiveProfile.FailedWorklogs)
                    {
                        collection.Add(status);
                    }
                    return collection;
                }

                return null;
            }
        }
        public ObservableCollection<Status> Statuses {
            get
            {
                if (HasFailedWorklogs)
                {
                    var collection = new ObservableCollection<Status>();
                    foreach(Status status in AppState.ActiveProfile.Statuses.Values)
                    {
                        collection.Add(status);
                    }
                    return collection;
                }

                return null;
            }
        }

        async private void CancelTracking()
        {
            AppState.Loader.Show();
            var result = await MsgBox.Show("Confirm", "Are you sure that you want to cancel tracking?", Icon.Stop, ButtonEnum.YesNo);
            if (result == ButtonResult.Yes)
            {
                AppState.ActiveProfile.CurrentTracking = null;
                AppState.Profiles.Save();
            }

            AppState.Loader.Hide();
        }

        async private void StartTracking()
        {
            if (IsTracking)
            {
                await MsgBox.Show("Error", "You are already tracking an issue. Please finish it first.", Icon.Warning);
                return;
            }

            //TODO move to appstate
            AppState.ActiveProfile.CurrentTracking = new Tracking()
            {
                Created = DateTime.Now,
                To = default(DateTime),
                IssueKey = SelectedIssue.IssueKey,
                Summary = SelectedIssue.Summary,
                Key = SelectedIssue.Key,
                ProjectKey = SelectedIssue.ProjectKey,
                Comment = "",
                Status = SelectedIssue.Status
            };

            AppState.Profiles.Save();
        }


        public void SelectIssue(Issue issue)
        {
            AppState.SelectIssue(issue);
        }


        async protected void RemoveFailedStatus()
        {
            AppState.Loader.Show();
            if (SelectedFailedStatus == null)
            {
                await MsgBox.Show("Error", "Select a failed status entry first.", Icon.Error, ButtonEnum.Ok);
            } else
            {
                //TODO move to AppState
                AppState.ActiveProfile.FailedStatusUpdates.RemoveAt(SelectedFailedIndex);
                AppState.Profiles.Save();
            }
            AppState.Loader.Hide();
        }

        public override void Init()
        {
            TimeUpdater = new Timer(1000);
            TimeUpdater.Elapsed += TimeUpdater_Elapsed;
            TimeUpdater.Start();
            base.Init();
        }

        private void OpenMantis()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(@"https://mantis.idct.pl/bug_report_page.php")
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        private void OpenFontAwesome()
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(@"https://fontawesome.com/license")
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        async protected void RetryFailedStatus()
        {
            if (SelectedFailedStatus == null)
            {
                await MsgBox.Show("Error", "Select a failed status entry first.", Icon.Error, ButtonEnum.Ok);
                AppState.Loader.Hide();
                return;
            }

            string issueKey = SelectedFailedStatus.IssueKey;
            string summary = SelectedFailedStatus.Summary;
            string status = SelectedFailedStatus.Status;

            _ = Task.Run(() => AppState.Jira.SetStatus(issueKey, summary, status).ContinueWith(
                async r => {
                    if (r.Result == false) {
                        await MsgBox.Show("Failure", "Failed to update the status", Icon.Error, ButtonEnum.Ok);
                    } else {
                        await MsgBox.Show("Success", "Status successfully updated", Icon.Success, ButtonEnum.Ok);
                        RemoveFailedStatus();
                    }
                    AppState.Loader.Hide();

            }));
        }

        public void SetStatus(bool saveOnFailure = false)
        {
            AppState.Loader.Show();

            string issueKey = SelectedIssue.Key;
            string summary = SelectedIssue.Summary;
            string status = SelectedNewStatus.Key;

            _ = Task.Run(() => AppState.Jira.SetStatus(issueKey, summary, status, saveOnFailure).ContinueWith(
                async r => {
                    if (r.Result == false) {
                        var result = await MsgBox.Show("Failed", "Failed to update the status. Do you wish to save the operation for later if next attempt fails [Yes], just retry [No] or abandon [Abort]?", Icon.Error, ButtonEnum.YesNoAbort);
                        if (result != ButtonResult.Abort) {
                            SetStatus(result == ButtonResult.Yes);
                        }
                        AppState.Loader.Hide();
                        return;
                    } else {
                        await MsgBox.Show("Success", "Status successfully updated", Icon.Success, ButtonEnum.Ok);
                        AppState.Loader.Hide();
                    }
            }));
        }

        async private void RemoveFailedWorklog()
        {
            AppState.Loader.Show();
            if (SelectedFailedWorklog == null)
            {
                await MsgBox.Show("Error", "Select a failed worklog entry first.", Icon.Error, ButtonEnum.Ok);
            } else
            {
                //TODO move to AppState
                AppState.ActiveProfile.FailedWorklogs.RemoveAt(SelectedFailedWorklogIndex);
                AppState.Profiles.Save();
            }
            AppState.Loader.Hide();
        }

        async protected void RetryFailedWorklog()
        {
            AppState.Loader.Show();
            if (SelectedFailedWorklog == null)
            {
                await MsgBox.Show("Error", "Select a failed worklog entry first.", Icon.Error, ButtonEnum.Ok);
                AppState.Loader.Hide();
                return;
            }

            _ = Task.Run(() => AppState.Jira.StoreWorklog(SelectedFailedWorklog).ContinueWith(
                async r => {
                    if (r.Result == false) {
                        await MsgBox.Show("Failure", "Failed to send the worklog", Icon.Error, ButtonEnum.Ok);
                    } else {
                        await MsgBox.Show("Success", "Worklog successfully saved", Icon.Success, ButtonEnum.Ok);
                        RemoveFailedWorklog();
                    }

                    AppState.Loader.Hide();
            }));
        }
    }
}