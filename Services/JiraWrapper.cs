using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Jira = Atlassian.Jira.Jira;
using JiraIssue = Atlassian.Jira.Issue;
using JiraStatus = Atlassian.Jira.IssueStatus;
using JiraProject = Atlassian.Jira.Project;
using JiraWorklog = Atlassian.Jira.Worklog;
using Sikor.Model;
using System.Security.Authentication;
using Sikor.Enum;
using Sikor.Container;
using Sikor.Util.Security;

namespace Sikor.Services
{
    public class JiraWrapper : ServiceProvider
    {
        Jira jira;

        AppState AppState;

        public override void Init()
        {
            AppState = ServiceContainer.GetServiceTyped<AppState>(typeof(AppState).ToString());
        }

        /**
         * <summary>
         * Stores the worklog
         * </summary>
         * <param name="issue"></param>
         * <param name="start"></param>
         * <param name="end"></param>
         * <param name="comment"></param>
         * <param name="saveOnFailure"></param>
         * <returns></returns>
         */
        async public Task<OperationResult> StoreWorklog(Tracking tracking, bool saveOnFailure = false)
        {
            try
            {
                if (tracking.To == default(DateTime))
                {
                    tracking.To = DateTime.Now;
                }

                var jiraIssue = await jira.Issues.GetIssueAsync(tracking.Key);
                double totalMinutes = Math.Round((tracking.To - tracking.Created).TotalMinutes);
                if (totalMinutes < 1)
                {
                    totalMinutes = 1;
                }

                string total = totalMinutes.ToString() + "m";
                //total = "143m";
                JiraWorklog worklog = new JiraWorklog(total, tracking.Created, tracking.Comment);
                // add a worklog
                await jiraIssue.AddWorklogAsync(worklog);
            }
            catch (Exception e)
            {
                if (!saveOnFailure)
                {
                    return OperationResult.FAILED;
                }

                AppState.ActiveProfile.FailedWorklogs.Add(tracking);
                AppState.Profiles.Save();
                return OperationResult.STORED;
            }

            return OperationResult.SAVED;

        }

        /**
         * <summary>
         * Sets the new status for an issue or keeps it for later.
         * </summary>
         * <param name="issueKey"></param>
         * <param name="summary"></param>
         * <param name="status"></param>
         * <param name="retry"></param>
         * <returns></returns>
         */
        async public Task<OperationResult> SetStatus(string issueKey, string summary, string status, bool saveOnFailure = false)
        {
            try
            {
                var issue = await jira.Issues.GetIssueAsync(issueKey);
                await issue.WorkflowTransitionAsync(status);
                await issue.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!saveOnFailure)
                {
                    return OperationResult.FAILED;
                }

                var failedStatusOperation = new FailedStatusUpdate()
                {
                    Created = DateTime.Now,
                    Key = issueKey,
                    Summary = summary,
                    Status = status
                };

                AppState.ActiveProfile.FailedStatusUpdates.Add(failedStatusOperation);
                AppState.Profiles.Save();
                return OperationResult.STORED;
            }

            return OperationResult.SAVED;
        }

        /**
         * <summary>
         * Attempts to perform a search requests, on any exception attempts to load from local cache.
         * </summary>
         * <param name="searchText"></param>
         * <param name="sorting"></param>
         * <param name="onlyCurrentUser"></param>
         * <param name="projectKey"></param>
         * <param name="statuses"></param>
         * <param name="profile"></param>
         * <returns></returns>
         */
        async public Task<SearchResults> Search(string searchText, string sorting, bool onlyCurrentUser, string projectKey, List<string> statuses, Profile profile)
        {
            try
            {
                var SearchParams = new SearchSettings()
                {
                    Summary = searchText,
                    Sorting = sorting,
                    AssignedToCurrentUser = onlyCurrentUser,
                    Project = projectKey,
                    Statuses = statuses
                };

                var results = await jira.Issues.GetIssuesFromJqlAsync(SearchParams.ToString());

                var issues = new ObservableCollection<Issue>();
                foreach (JiraIssue issue in results)
                {
                    Issue item = new Issue();
                    item.Status = issue.Status.Name;
                    item.ProjectKey = issue.Project;
                    item.Key = issue.Key.Value;
                    item.Value = issue.Summary;
                    item.Summary = issue.Summary;
                    item.Type = issue.Type.Name;
                    item.TimeSpent = issue.TimeTrackingData.TimeSpent;
                    issues.Add(item);
                    profile.Issues[item.Key] = item;
                }

                var searchResults = new SearchResults()
                {
                    Offline = false,
                    Issues = issues
                };

                return searchResults;
            }
            catch (Exception e)
            {
                var issues = new ObservableCollection<Issue>();
                foreach (Issue item in profile.Issues.Values)
                {

                    if (projectKey.Length > 0 && item.ProjectKey != projectKey)
                    {
                        continue;
                    }

                    if (statuses.Count > 0 && !statuses.Contains(item.Status))
                    {
                        continue;
                    }

                    if (!item.Value.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    issues.Add(item);
                }

                var searchResults = new SearchResults()
                {
                    Offline = true,
                    Issues = issues
                };


                return searchResults;
            }

        }

        /**
         * <summary>
         * Attempts to create a new profile.
         * </summary>
         * <param name="profileName"></param>
         * <param name="url"></param>
         * <param name="username"></param>
         * <param name="password"></param>
         * <returns></returns>
         */
        async public Task<LoginState> CreateProfile(string profileName, string url, string username, string password)
        {
            if (profileName.Length == 0)
            {
                throw new ArgumentException("Profile name must a non-empty string.");
            }

            if (AppState.Profiles.Has(HashGenerator.MD5(profileName)))
            {
                throw new ArgumentException("Profile with such name already exists.");
            }

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                throw new ArgumentException("Invalid URL format. Please use full URL starting with http or https.");
            }

            if (username.Length == 0 || password.Length == 0)
            {
                throw new ArgumentException("Password and username must not be empty.");
            }

            var profile = new Profile()
            {
                Name = profileName,
                Password = password,
                Uri = url,
                Username = username
            };

            LoginState loginResult = await Login(profile);

            if (loginResult == LoginState.SUCCESS)
            {
                AppState.Profiles.Add(profile);
                AppState.Profiles.Save();

                AppState.Settings.LastSelectedProfile = profile.GetId();
                AppState.Settings.Save();
            }

            return loginResult;
        }

        async public Task<LoginState> Login(Profile profile)
        {
            //opens new connection
            jira = Jira.CreateRestClient(profile.Uri, profile.Username, profile.Password);
            jira.RestClient.RestSharpClient.Timeout = 3000;
            try
            {
                //tries to load projects
                var projects = await jira.Projects.GetProjectsAsync();
                profile.Projects.Clear();
                foreach (JiraProject project in projects)
                {
                    var newProject = new Project();
                    newProject.Key = project.Key;
                    newProject.Value = project.Name;
                    profile.Projects.Add(project.Id, newProject);
                }

                //attempt to load statuses possible in the jira
                var statuses = await jira.Statuses.GetStatusesAsync();
                profile.Statuses.Clear();
                foreach (JiraStatus status in statuses)
                {
                    var issueStatusItem = new Status();
                    issueStatusItem.Key = status.Name;
                    issueStatusItem.Value = status.Name;
                    profile.Statuses.Add(status.Id, issueStatusItem);
                }
            }
            catch (AuthenticationException e)
            {
                return LoginState.INVALID_CREDENTIALS;
            }
            catch (Exception e)
            {
                return LoginState.NETWORK_ERROR;
            }

            //store any updates to the profile
            AppState.Profiles.Save();

            //mark the last selected profile in settings
            AppState.Settings.LastSelectedProfile = profile.GetId();
            AppState.Settings.Save();

            return LoginState.SUCCESS;
        }
    }
}
