using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
//using Atlassian.Jira;
using Sikor.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sikor.Model;
using Sikor.Repository;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia;
using Sikor.Util;
using Sikor.Container;

namespace Sikor.ViewModels
{
    public class SidebarViewModel : ReactiveViewServiceProvider
    {
        public ObservableCollection<Status> Statuses {
            get {
                var statuses = new ObservableCollection<Status>();
                foreach(Status status in AppState.ActiveProfile.Statuses.Values) {
                    statuses.Add(status);
                }

                return statuses;
            }
        }

        public bool SearchOnlyCurrentUsersIssues { get; set; }
        public ObservableCollection<Issue> Issues { get; private set; }

        public Issue SelectedIssue
        {
            get
            {
                return AppState.ActiveProfile.SelectedIssue;
            }
            set
            {
                AppState.SelectIssue(value);
            }
        }
        public ObservableCollection<Project> Projects {
            get {
                var profileProjects = new ObservableCollection<Project>();

                Project Any = new Project()
                {
                    Key = "-1",
                    Value = "-- any --",
                    Shortcut = ""
                };

                profileProjects.Add(Any);

                if (SelectedProject == null || !AppState.ActiveProfile.Projects.ContainsKey(SelectedProject.Key)) {
                    SelectedProject = Any;
                }

                foreach (Project project in AppState.ActiveProfile.Projects.Values)
                {
                    profileProjects.Add(project);
                }

                return profileProjects;
            }
        }
        public ListableItem SelectedSorting { get; private set; }
        public ObservableCollection<ListableItem> SortOptions { get {
            //not selected
            var first = new ListableItem() {
                    Value = "Last viewed, desc",
                    Key = "LastViewed_DESC"
                };

            if (SelectedSorting == null) {
                SelectedSorting = first;
            }

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
        } }

        protected string searchString;
        public string IssueSearchText {
            get { return searchString; }
            set {
                searchString = value;
                if (searchString.Length > 2)
                {
                    search();
                }
            }
        }

        async protected void search()
        {
            var selectedStatuses = new List<string>();
            foreach (Status status in AppState.ActiveProfile.Statuses.Values)
            {
                if (status.Selected)
                {
                    selectedStatuses.Add(status.Key);
                }
            }

            Issues = await AppState.Jira.Search(searchString, SelectedSorting.Key, SearchOnlyCurrentUsersIssues, SelectedProject.Key != "-1" ? SelectedProject.Key : "", selectedStatuses, AppState.ActiveProfile);
            this.RaisePropertyChanged("Issues");
        }


        public Project SelectedProject { get; set; }

        public void Reload()
        {
            SearchOnlyCurrentUsersIssues = false;
            searchString = "";

            this.RaisePropertyChanged("SortOptions");
            this.RaisePropertyChanged("Issues");
            this.RaisePropertyChanged("Projects");
            this.RaisePropertyChanged("SelectedProject");
            this.RaisePropertyChanged("IssueStatuses");

        }
    }
}
