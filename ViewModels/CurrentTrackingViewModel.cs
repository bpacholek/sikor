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
using Sikor.Util.Ui;
using System.Timers;
using System.Diagnostics;

namespace Sikor.ViewModels
{

    public class CurrentTrackingViewModel : ReactiveObject, IService
    {
        public bool IsSelected { get; set; }
        public bool IsTracking
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    return userState.UserProfile.IsTracking;
                }
                return false;
            }
        }

        public bool HasFailedStatuses
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    return userState.UserProfile.FailedStatusUpdateOperations.Count > 0;
                }
                return false;
            }
        }

        public bool HasFailedWorklogs
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    return userState.UserProfile.FailedWorklogs.Count > 0;
                }
                return false;
            }
        }

        async public void StoreTracking()
        {
            Loader.Show();
            if ((DateTime.Now - TrackedIssue.Created).TotalMinutes < 1)
            {
                var result = await MsgBox.Show("Warning", "Sadly, Jira cannot track anything under a minute. Do you wish to save this tracking as 1 minute [Yes] or continue tracking [Cancel]?", Icon.Warning, ButtonEnum.OkCancel);
                if (result == ButtonResult.Cancel)
                {
                    Loader.Hide();
                    return;
                }
            }

            jira.StoreWorklog(TrackedIssue.Key, TrackedIssue.Created, DateTime.Now, TrackedIssue.Comment);
        }

        public TrackingIssueItem TrackedIssue
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    return userState.UserProfile.TrackingIssue;
                }
                return null;
            }
        }

        public IssueItem SelectedIssue { get; private set; }

        public IssueStatusItem SelectedNewStatus {get;set;}

        public ObservableCollection<FailedStatusUpdateOperation> FailedStatuses
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    var collection = new ObservableCollection<FailedStatusUpdateOperation>();
                    foreach(FailedStatusUpdateOperation status in userState.UserProfile.FailedStatusUpdateOperations)
                    {
                        collection.Add(status);
                    }
                    return collection;
                }
                return null;
            }
        }

        public ObservableCollection<FailedWorklog> FailedWorklogs
        {
            get
            {
                if (userState != null && userState.UserProfile != null)
                {
                    var collection = new ObservableCollection<FailedWorklog>();
                    foreach (FailedWorklog worklog in userState.UserProfile.FailedWorklogs)
                    {
                        collection.Add(worklog);
                    }
                    return collection;
                }
                return null;
            }
        }
        public ObservableCollection<IssueStatusItem> IssueStatuses { get; private set; }


        public void SelectIssue(IssueItem issue)
        {
            SelectedIssue = issue;
            IsSelected = true;
            this.RaisePropertyChanged("IsSelected");
            this.RaisePropertyChanged("SelectedIssue");
        }
        public CurrentTrackingViewModel()
        {
            IsSelected = false;
            //IsTracking = false;

            ServicesContainer.RegisterService("current_tracking", this);
        }

        protected UserState userState;

        protected FullLoaderViewModel Loader;

        public FailedStatusUpdateOperation SelectedFailedStatus { get; set; }

        public FailedWorklog SelectedFailedWorklog { get; set; }


        public int SelectedFailedWorklogIndex { get; set; }

        public int SelectedFailedIndex { get; set; }

        async protected void RemoveFailedStatus()
        {
            Loader.Show();
            if (SelectedFailedStatus == null)
            {
                await MsgBox.Show("Error", "Select a failed status entry first.", Icon.Error, ButtonEnum.Ok);
            } else
            {
                userState.UserProfile.FailedStatusUpdateOperations.RemoveAt(SelectedFailedIndex);
                ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
                this.RaisePropertyChanged("HasFailedStatuses");
                this.RaisePropertyChanged("FailedStatuses");
                this.RaisePropertyChanged("FailedStatusesList");
            }
            Loader.Hide();
        }

        async protected void RetryFailedStatus()
        {
            Loader.Show();
            if (SelectedFailedStatus == null)
            {
                await MsgBox.Show("Error", "Select a failed status entry first.", Icon.Error, ButtonEnum.Ok);
                Loader.Hide();

            }
            else
            {
                string issueKey = SelectedFailedStatus.IssueKey;
                string summary = SelectedFailedStatus.Summary;
                string status = SelectedFailedStatus.Status;
                jira.SetStatus(issueKey, summary, status, true);
            }

        }

        async public void SetStatus()
        {
            string issueKey = SelectedIssue.Key;
            string summary = SelectedIssue.Name;
            string status = SelectedNewStatus.Key;

            Loader.Show();
            jira.SetStatus(issueKey, summary, status);
        }

        JiraWrapper jira;

        public void init()
        {
            userState = ServicesContainer.GetServiceTyped<UserState>("state");
            Loader = ServicesContainer.GetServiceTyped<FullLoaderViewModel>("loader");
            jira = ServicesContainer.GetServiceTyped<JiraWrapper>("signed_jira");
            TimeUpdater = new Timer(1000);
            TimeUpdater.Elapsed += TimeUpdater_Elapsed;
            TimeUpdater.Start();
            jira.onStatusUpdateSuccess += Jira_onStatusUpdateSuccess;
            jira.onStatusUpdateFailed += Jira_onStatusUpdateFailed;
            jira.onStatusUpdateRetryFailed += Jira_onStatusUpdateRetryFailed;
            jira.onStatusUpdateRetrySuccess += Jira_onStatusUpdateSuccess1;

            jira.onWorklogAddedSuccessful += Jira_onWorklogAddedSuccessful;
            jira.onWorklogAddingFailed += Jira_onWorklogAddingFailed;
            jira.onWorklogRetryAddedSuccessful += Jira_onWorklogRetryAddedSuccessful;
            jira.onWorklogRetryAddingFailed += Jira_onWorklogRetryAddingFailed;

            IssueStatuses = new ObservableCollection<IssueStatusItem>();
            foreach (IssueStatusItem status in this.userState.UserProfile.Statuses.Values)
            {
                IssueStatuses.Add(status);
            }
            this.RaisePropertyChanged("IssueStatuses");
            this.RaisePropertyChanged("TrackedIssue");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("TrackedIssue_Project");
            this.RaisePropertyChanged("TrackedIssue_Status");
            this.RaisePropertyChanged("TrackedIssue_Comment");
            this.RaisePropertyChanged("TrackedIssue_Created");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("IsTracking");
            this.RaisePropertyChanged("HasFailedStatuses");
            this.RaisePropertyChanged("FailedStatuses");
            this.RaisePropertyChanged("HasFailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogsList");

        }

        async private void Jira_onWorklogRetryAddingFailed(string issueKey, DateTime start, DateTime end, string comment)
        {
            MsgBox.Show("Worklog", "Failed to store the worklog", Icon.Error, ButtonEnum.Ok);
            Loader.Hide();
        }

        async private void Jira_onWorklogRetryAddedSuccessful(Issue issue)
        {
            userState.UserProfile.FailedWorklogs.RemoveAt(SelectedFailedWorklogIndex);
            ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
            this.RaisePropertyChanged("HasFailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogsList");
            await MsgBox.Show("Error", "Status update successful; issue " + issue.Key, Icon.Success, ButtonEnum.Ok);
            Loader.Hide();
        }

        async private void RemoveFailedWorklog()
        {
            Loader.Show();
            if (SelectedFailedWorklog == null)
            {
                await MsgBox.Show("Error", "Select a failed worklog entry first.", Icon.Error, ButtonEnum.Ok);
            }
            else
            {
                userState.UserProfile.FailedWorklogs.RemoveAt(SelectedFailedWorklogIndex);
                ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
                this.RaisePropertyChanged("HasFailedWorklogs");
                this.RaisePropertyChanged("FailedWorklogs");
                this.RaisePropertyChanged("FailedWorklogsList");
            }
            Loader.Hide();
        }

        async private void RetryFailedWorklog()
        {
            Loader.Show();
            if (SelectedFailedWorklog == null)
            {
                await MsgBox.Show("Error", "Select a failed worklog entry first.", Icon.Error, ButtonEnum.Ok);
                Loader.Hide();
            }
            else
            {
                jira.StoreWorklog(SelectedFailedWorklog.IssueKey, SelectedFailedWorklog.From, SelectedFailedWorklog.To, SelectedFailedWorklog.Comment, true);
            }

        }

        async private void Jira_onWorklogAddingFailed(string issueKey, DateTime start, DateTime end, string comment)
        {
            var result = await MsgBox.Show("Failed", "Failed to store the worklog. Do you wish to retry the attempt now [Yes], store it for later [No] or abandon [Abort]?", Icon.Error, ButtonEnum.YesNoAbort);

            if (result == ButtonResult.Yes)
            {
                jira.StoreWorklog(TrackedIssue.Key, TrackedIssue.Created, DateTime.Now, TrackedIssue.Comment);
                return;
            }

            if (result == ButtonResult.Abort)
            {
                userState.UserProfile.IsTracking = false;
                userState.UserProfile.TrackingIssue = null;
                ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
                this.RaisePropertyChanged("TrackedIssue");
                this.RaisePropertyChanged("TrackedIssue_Name");
                this.RaisePropertyChanged("TrackedIssue_Project");
                this.RaisePropertyChanged("TrackedIssue_Status");
                this.RaisePropertyChanged("TrackedIssue_Comment");
                this.RaisePropertyChanged("TrackedIssue_Created");
                this.RaisePropertyChanged("TrackedIssue_Name");
                this.RaisePropertyChanged("IsTracking");
                Loader.Hide();
                return;
            }


            var FailedWorklog = new FailedWorklog();
            FailedWorklog.Created = TrackedIssue.Created;
            FailedWorklog.Comment = TrackedIssue.Comment;
            FailedWorklog.To = DateTime.Now;
            FailedWorklog.Summary = TrackedIssue.Name;
            FailedWorklog.IssueKey = TrackedIssue.Key;

            userState.UserProfile.IsTracking = false;
            userState.UserProfile.TrackingIssue = null;

            userState.UserProfile.FailedWorklogs.Add(FailedWorklog);
            userState.UserProfile.IsTracking = false;
            userState.UserProfile.TrackingIssue = null;
            this.RaisePropertyChanged("TrackedIssue");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("TrackedIssue_Project");
            this.RaisePropertyChanged("TrackedIssue_Status");
            this.RaisePropertyChanged("TrackedIssue_Comment");
            this.RaisePropertyChanged("TrackedIssue_Created");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("TrackedIssue_Name");

            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("HasFailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogs");
            this.RaisePropertyChanged("FailedWorklogsList");
            this.RaisePropertyChanged("IsTracking");
            ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();


            Loader.Hide();
        }

        async private void Jira_onWorklogAddedSuccessful(Issue issue)
        {
            userState.UserProfile.IsTracking = false;
            userState.UserProfile.TrackingIssue = null;
            ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
            this.RaisePropertyChanged("TrackedIssue");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("TrackedIssue_Project");
            this.RaisePropertyChanged("TrackedIssue_Status");
            this.RaisePropertyChanged("TrackedIssue_Comment");
            this.RaisePropertyChanged("TrackedIssue_Created");
            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("IsTracking");

            await MsgBox.Show("Success", "Worklog successfully saved", Icon.Success, ButtonEnum.Ok);
            Loader.Hide();
        }

        async private void Jira_onStatusUpdateSuccess1(Issue issue)
        {
            userState.UserProfile.FailedStatusUpdateOperations.RemoveAt(SelectedFailedIndex);
            ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
            this.RaisePropertyChanged("HasFailedStatuses");
            this.RaisePropertyChanged("FailedStatuses");
            this.RaisePropertyChanged("FailedStatusesList");
            await MsgBox.Show("Error", "Status update successful; issue " + issue.Key, Icon.Success, ButtonEnum.Ok);
            Loader.Hide();
        }

        async private void Jira_onStatusUpdateRetryFailed(string issueKey, string summary, string status)
        {
            await MsgBox.Show("Error", "Status update has failed for issue " + issueKey, Icon.Error, ButtonEnum.Ok);
            Loader.Hide();
        }

        private void TimeUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsTracking)
            {
                var diff = DateTime.Now - userState.UserProfile.TrackingIssue.Created;
                TrackingTime = Math.Floor(diff.TotalHours).ToString().PadLeft(2, '0') + ":" + diff.Minutes.ToString().PadLeft(2,'0') + ":" + diff.Seconds.ToString().PadLeft(2,'0');
                this.RaisePropertyChanged("TrackingTime");

            }
        }

        public string TrackingTime { get; set; }

        Timer TimeUpdater;

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

        async private void StartTracking()
        {
            if (userState.UserProfile.IsTracking)
            {
                await MsgBox.Show("Error", "You are already tracking an issue. Please finish it first.", Icon.Warning);
                return;
            }
                userState.UserProfile.IsTracking = true;
            var trackedIssue = new TrackingIssueItem();
            TrackingTime = "0s";
            trackedIssue.Created = DateTime.Now;
            trackedIssue.Key = SelectedIssue.Key;
            trackedIssue.Name = SelectedIssue.Name;
            trackedIssue.Project = SelectedIssue.Project;
            trackedIssue.Status = SelectedIssue.Status;
            trackedIssue.Comment = "";




            userState.UserProfile.TrackingIssue = trackedIssue;

            ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
            this.RaisePropertyChanged("TrackedIssue");

            this.RaisePropertyChanged("TrackedIssue_Name");
            this.RaisePropertyChanged("TrackedIssue_Project");

            this.RaisePropertyChanged("TrackedIssue_Status");

            this.RaisePropertyChanged("TrackedIssue_Comment");

            this.RaisePropertyChanged("TrackedIssue_Created");
            this.RaisePropertyChanged("TrackedIssue_Name");


            this.RaisePropertyChanged("IsTracking");
        }

        async private void CancelTracking()
        {
            Loader.Show();
            var result = await MsgBox.Show("Confirm", "Are you sure that you want to cancel tracking?", Icon.Stop, ButtonEnum.YesNo);
            if (result == ButtonResult.No)
            {
                //do nothing
            }
            else
            {
                userState.UserProfile.IsTracking = false;
                userState.UserProfile.TrackingIssue = null;
                ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
                this.RaisePropertyChanged("TrackedIssue");
                this.RaisePropertyChanged("TrackedIssue_Name");
                this.RaisePropertyChanged("TrackedIssue_Project");
                this.RaisePropertyChanged("TrackedIssue_Status");
                this.RaisePropertyChanged("TrackedIssue_Comment");
                this.RaisePropertyChanged("TrackedIssue_Created");
                this.RaisePropertyChanged("TrackedIssue_Name");
                this.RaisePropertyChanged("IsTracking");


            }

            Loader.Hide();
        }
        async private void Jira_onStatusUpdateFailed(string issueKey, string summary, string status)
        {
            var result = await MsgBox.Show("Status update failure", "Failed to save the status update operation. Would you like to keep it for later?", Icon.Warning, ButtonEnum.YesNo);
            if (result == ButtonResult.No)
            {
                //do nothing
            }
            else
            {
                var failedStatusOperation = new FailedStatusUpdateOperation();
                failedStatusOperation.Created = DateTime.Now;
                failedStatusOperation.IssueKey = issueKey;
                failedStatusOperation.Summary = summary;
                failedStatusOperation.Status = status;

                userState.UserProfile.FailedStatusUpdateOperations.Add(failedStatusOperation);
                ServicesContainer.GetServiceTyped<Profiles>("profiles").Save();
                this.RaisePropertyChanged("HasFailedStatuses");
                this.RaisePropertyChanged("FailedStatuses");

            }
            Loader.Hide();
        }

        async private void Jira_onStatusUpdateSuccess(Issue issue)
        {
            var result = await MsgBox.Show("Status update success", "Status successfully update", Icon.Success, ButtonEnum.Ok);
            Loader.Hide();
        }
    }
}