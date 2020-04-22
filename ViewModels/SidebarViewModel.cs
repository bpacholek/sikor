using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sikor.Services;
using Sikor.Model;
using Sikor.Container;
using System.Threading.Tasks;

namespace Sikor.ViewModels
{
    public class SidebarViewModel : ReactiveViewServiceProvider
    {
        public ObservableCollection<Status> Statuses
        {
            get
            {
                if (AppState.ActiveProfile == null)
                {
                    return null;
                }
                var statuses = new ObservableCollection<Status>();
                foreach (Status status in AppState.ActiveProfile.Statuses.Values)
                {
                    statuses.Add(status);
                }

                return statuses;
            }
        }

        public bool SearchOnlyCurrentUsersIssues { get; set; }
        public ObservableCollection<Issue> Issues { get; private set; }

        protected AnonymousToken searchCancelToken;
        public Issue SelectedIssue
        {
            get
            {
                return AppState.ActiveProfile != null ? AppState.ActiveProfile.SelectedIssue : null;
            }
            set
            {
                AppState.SelectIssue(value);
            }
        }
        public ObservableCollection<Project> Projects
        {
            get
            {
                if (AppState.ActiveProfile == null)
                {
                    return null;
                }

                var profileProjects = new ObservableCollection<Project>();

                Project Any = new Project()
                {
                    Key = "-1",
                    Value = "-- any --"
                };
                profileProjects.Add(Any);

                if (SelectedProject == null || !AppState.ActiveProfile.Projects.ContainsKey(SelectedProject.Key))
                {
                    SelectedProject = Any;
                }

                foreach (Project project in AppState.ActiveProfile.Projects.Values)
                {
                    profileProjects.Add(project);
                }

                return profileProjects;
            }
        }

        protected ListableItem selectedSorting;

        public ListableItem SelectedSorting
        {
            get => selectedSorting;
            set => this.RaiseAndSetIfChanged(ref selectedSorting, value);
        }
        public ObservableCollection<ListableItem> SortOptions
        {
            get
            {
                //not selected
                var first = new ListableItem()
                {
                    Value = "Last viewed, desc",
                    Key = "LastViewed_DESC"
                };

                selectedSorting = first;

                return new ObservableCollection<ListableItem> {
                first,
                new ListableItem() {
                    Value = "Last viewed, asc",
                    Key = "LastViewed_ASC"
                },
                new ListableItem() {
                    Value = "Created, desc",
                    Key = "Created_DESC"
                },
                new ListableItem() {
                    Value = "Created, asc",
                    Key = "Created_ASC"
                },
                new ListableItem() {
                    Value = "Time spent, desc",
                    Key = "TimeSpent_DESC"
                },
                new ListableItem() {
                    Value = "Time spent, asc",
                    Key = "TimeSpent_asc"
                }
            };
            }
        }

        public string SearchClass { get; private set; }
        public string SearchText { get; private set; }


        protected string searchString;
        public string IssueSearchText
        {
            get { return searchString; }
            set
            {
                searchString = value;
                if (searchString.Length > 2)
                {
                    Search();
                }
            }
        }

        public void Search()
        {
            if (searchCancelToken != null)
            {
                searchCancelToken.Valid = false;
            }

            var selectedStatuses = new List<string>();
            foreach (Status status in AppState.ActiveProfile.Statuses.Values)
            {
                if (status.Selected)
                {
                    selectedStatuses.Add(status.Key);
                }
            }
            UpdateSearchProperties("Searching", "Searching");

            searchCancelToken = new AnonymousToken() {
                Valid = true
            };

            _ = Task.Run(() => AppState.Jira.Search(searchString, SelectedSorting.Key, SearchOnlyCurrentUsersIssues, SelectedProject.Key != "-1" ? SelectedProject.Key : "", selectedStatuses, AppState.ActiveProfile, searchCancelToken).ContinueWith(
                r =>
                {
                    if (r.Result.Valid) {
                        Issues = r.Result.Issues;
                        UpdateSearchProperties("Results", "Finished " + (r.Result.Offline ? "OFFLINE" : "ONLINE"));
                    }
                }));
        }

        protected void UpdateSearchProperties(string style, string text)
        {
            SearchClass = style;
            SearchText = text;
            this.RaisePropertyChanged("SearchClass");
            this.RaisePropertyChanged("SearchText");
            this.RaisePropertyChanged("Issues");
        }
        public Project SelectedProject { get; set; }

        public override void PostInit()
        {
            SearchOnlyCurrentUsersIssues = false;
            searchString = "";

            UpdateSearchProperties("Idle", "Idle");

            this.RaisePropertyChanged("SortOptions");
            this.RaisePropertyChanged("SelectedSorting");
            this.RaisePropertyChanged("Issues");
            this.RaisePropertyChanged("Projects");
            this.RaisePropertyChanged("SelectedProject");
            this.RaisePropertyChanged("Statuses");
        }
    }
}
