using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using Newtonsoft.Json.Linq;
using Sikor.Model;
using Sikor.Repository;

namespace Sikor.Services
{
    public class JiraWrapper : IService
    {
        Jira jira;

        Profiles Profiles;

        public delegate void LoginResponse();

        public delegate void UpdateSuccessful(Issue issue);
        public delegate void StatusUpdateFailed(string issueKey, string summary, string status);

        public delegate void WorklogAdded(Issue issue);
        public delegate void WorklogFailed(string issueKey, DateTime start, DateTime end, string comment);

        public event WorklogAdded onWorklogAddedSuccessful;
        public event WorklogAdded onWorklogRetryAddedSuccessful;

        public event WorklogFailed onWorklogAddingFailed;
        public event WorklogFailed onWorklogRetryAddingFailed;


        public event StatusUpdateFailed onStatusUpdateFailed;
        public event UpdateSuccessful onStatusUpdateSuccess;

        public event StatusUpdateFailed onStatusUpdateRetryFailed;
        public event UpdateSuccessful onStatusUpdateRetrySuccess;

        public delegate void ValidationError(string message);

        public event LoginResponse onInvalidCredentials;
        public event LoginResponse onNetworkIssue;
        public event LoginResponse onSuccessfulLogin;
        public event ValidationError onValidationError;

        public JiraWrapper()
        {
            Profiles = ServicesContainer.GetServiceTyped<Profiles>("profiles");
        }

        async public Task<bool> StoreWorklog(string issueKey, DateTime start, DateTime end, string comment, bool retry = false)
        {
            try
            {
                var issue = await jira.Issues.GetIssueAsync(issueKey);

                double totalMinutes = Math.Round((end - start).TotalMinutes);
                if (totalMinutes < 1)
                {
                    totalMinutes = 1;
                }

                string total = totalMinutes.ToString() + "m";
                //total = "143m";
                Worklog worklog = new Worklog(total, start, comment);
                // add a worklog
                await issue.AddWorklogAsync(worklog);
                if (!retry)
                {
                    onWorklogAddedSuccessful(issue);
                } else
                {
                    onWorklogRetryAddedSuccessful(issue);
                }
            }
            catch (Exception e)
            {
                if (!retry)
                {
                    onWorklogAddingFailed(issueKey, start, end, comment);
                }
                else
                {
                    onWorklogRetryAddingFailed(issueKey, start, end, comment);
                }
            }

            return true;

        }
        async public Task<bool> SetStatus(string issueKey, string summary, string status, bool retry = false)
        {
            try
            {
                var issue = await jira.Issues.GetIssueAsync(issueKey);
                await issue.WorkflowTransitionAsync(status);
                await issue.SaveChangesAsync();
                if (retry)
                {
                    onStatusUpdateRetrySuccess(issue);
                }
                else
                {
                    onStatusUpdateSuccess(issue);
                }
            }
            catch (Exception e)
            {
                if (retry)
                {
                    onStatusUpdateRetryFailed(issueKey, summary, status); 
                } else
                {
                    onStatusUpdateFailed(issueKey, summary, status);
                }
            }


            return true;
        }
 
        async public Task<ObservableCollection<IssueItem>> Search(string searchText, string sorting, bool onlyCurrentUser, string projectKey, List<string> statuses, Profile profile)
        {
            try
            {
                string jql = "Summary ~ \"%" + searchText + "%\"";

                if (onlyCurrentUser)
                {
                    jql += " && assignee = currentUser()";
                }

                if (projectKey.Length > 0)
                {
                    jql += " && project = " + projectKey;
                }

                if (statuses.Count > 0)
                {
                    jql += " && status in (\"" + String.Join("\",\"", statuses) + "\")";
                }

                if (sorting.Length > 0)
                {
                    jql += " ORDER BY " + sorting;
                }


                var results = await jira.Issues.GetIssuesFromJqlAsync(jql);

                var output = new ObservableCollection<IssueItem>();
                foreach (Issue issue in results)
                {
                    IssueItem item = new IssueItem();
                    item.Status = issue.Status.Name;
                    item.Project = issue.Project;
                    item.Key = issue.Key.Value;
                    item.Name = issue.Summary;
                    output.Add(item);
                    profile.Issues[item.Key] = item;
                }

                return output;
            }
            catch (System.Security.Authentication.AuthenticationException e)
            {
                onInvalidCredentials();
                return new ObservableCollection<IssueItem>();
            }
            catch (Exception e)
            {
                //onNetworkIssue();
                var output = new ObservableCollection<IssueItem>();
                foreach (IssueItem item in profile.Issues.Values)
                {

                    if (projectKey.Length > 0 && item.Project != projectKey)
                    {
                        continue;
                    }

                    if (statuses.Count > 0 && !statuses.Contains(item.Status))
                    {
                        continue;
                    }

                    if (!item.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue; 
                    }

                    output.Add(item);
                }

                return output;
            }

        }

        async public Task<bool> CreateProfile(string profileName, string hostname, string username, string password)
        {
            JiraRestClientSettings settings = new JiraRestClientSettings();
            Uri uri;

            if (profileName.Length == 0)
            {
                onValidationError("Profile name must a non-empty string.");
                return false;
            }

            if (Profiles.Has(Profile.CalculateMD5Hash(profileName)))
            {
                onValidationError("Profile with such name already exists.");
                return false;
            }

            if (!Uri.TryCreate(hostname, UriKind.Absolute, out uri))
            {
                onValidationError("Invalid URL format. Please use full URL starting with http or https.");
                return false;
            }

            if (username.Length == 0 || password.Length == 0)
            {
                onValidationError("Password and username must not be empty.");
                return false;
            }

            jira = Jira.CreateRestClient(hostname, username, password);
            Profile profile = new Profile();

            jira.RestClient.RestSharpClient.Timeout = 3000;
            try
            {
                var projects = await jira.Projects.GetProjectsAsync();
                profile.Projects.Clear();
                foreach (Project project in projects)
                {
                    var newProject = new ListItem();
                    newProject.Key = project.Key;
                    newProject.Name = project.Name;
                    newProject.Value = project.Name;
                    profile.Projects.Add(project.Id, newProject);
                }

                var statuses = await jira.Statuses.GetStatusesAsync();
                profile.Statuses.Clear();
                foreach (IssueStatus status in statuses)
                {
                    var issueStatusItem = new IssueStatusItem();
                    issueStatusItem.Key = status.Name;
                    issueStatusItem.Name = status.Name;
                    issueStatusItem.Value = status.Name;
                    profile.Statuses.Add(status.Id, issueStatusItem);
                }
            }
            catch (System.Security.Authentication.AuthenticationException e)
            {
                onInvalidCredentials();
                return false;
            }
            catch (Exception e)
            {
                onNetworkIssue();
                return false;
            }

            profile.Name = profileName;
            profile.Password = password;
            profile.Host = hostname;
            profile.Username = username;

            Profiles.Add(profile);
            Profiles.Save();

            var appSettings = ServicesContainer.GetServiceTyped<Settings>("settings");

            appSettings.LastSelectedProfile = profile.GetId();
            appSettings.Save();
            onSuccessfulLogin();
            return true;
        }

        async public Task<bool> AtteptLogin(Profile profile)
        {
            JiraRestClientSettings settings = new JiraRestClientSettings();

            jira = Jira.CreateRestClient( profile.Host, profile.Username, profile.Password, settings );

            ServicesContainer.RegisterService("signed_jira", this);
            jira.RestClient.RestSharpClient.Timeout = 3000;
            try
            {
                var projects = await jira.Projects.GetProjectsAsync();
                profile.Projects.Clear();
                foreach (Project project in projects)
                {
                    var newProject = new ListItem();
                    newProject.Key = project.Key;
                    newProject.Name = project.Name;
                    newProject.Value = project.Name;
                    profile.Projects.Add(project.Id, newProject);
                }

                var statuses = await jira.Statuses.GetStatusesAsync();
                profile.Statuses.Clear();
                foreach (IssueStatus status in statuses)
                {
                    var issueStatusItem = new IssueStatusItem();
                    issueStatusItem.Key = status.Name;
                    issueStatusItem.Name = status.Name;
                    issueStatusItem.Value = status.Name;
                    profile.Statuses.Add(status.Id, issueStatusItem);
                }
            }
            catch (System.Security.Authentication.AuthenticationException e)
            {
                onInvalidCredentials();
                return false;
            }
            catch (Exception e)
            {
                onNetworkIssue();
                return false;
            }

            Profiles.Save();

            var appSettings = ServicesContainer.GetServiceTyped<Settings>("settings");


            appSettings.LastSelectedProfile = profile.GetId();
            appSettings.Save();
            onSuccessfulLogin();
            return true;
        }
    }
}

        /*
        async protected Task<bool> attemptLogin(string hostname, string username, string password, bool newProject = false)
        {
            try
            {
                JiraRestClientSettings settings = new JiraRestClientSettings();
                jira = Jira.CreateRestClient(hostname, username, password);
                var d = await jira.Projects.GetProjectsAsync();

                Sikor.Model.Profile project = new Sikor.Model.Profile();
                project.Host = hostname;
                project.Password = password;
                project.Username = username;
                if (newProject)
                {
                    project.Name = ProfileName;
                    profiles.Add(project);
                    storage.Set("profiles", profiles);
                }

                this.RaisePropertyChanged("TrackedIssue");

                LoginFormVisible = false;
                state = State.SEARCH;
                loadUserProjects();
                loadStatuses();
                loadTasks();
            }
            catch (System.Security.Authentication.AuthenticationException e)
            {
                Error = "Invalid credentials";
            }
            catch (Exception e)
            {
                Error = e.Message;
            }

            return true;
        }
    }
}
*/