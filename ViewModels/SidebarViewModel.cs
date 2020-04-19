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

        public ListableItem SelectedSorting {
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

        protected string searchString;
        public string IssueSearchText
        {
            get { return searchString; }
            set
            {
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

        public override void PostInit()
        {
            SearchOnlyCurrentUsersIssues = false;
            searchString = "";

            this.RaisePropertyChanged("SortOptions");
            this.RaisePropertyChanged("SelectedSorting");

            this.RaisePropertyChanged("Issues");
            this.RaisePropertyChanged("Projects");
            this.RaisePropertyChanged("SelectedProject");
            this.RaisePropertyChanged("Statuses");
        }
    }
}
