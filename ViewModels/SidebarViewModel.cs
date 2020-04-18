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
using Sikor.Util;
namespace Sikor.ViewModels
{
    public class SidebarViewModel : ReactiveObject, IService
    {
        protected UserState userState;

        JiraWrapper signedJira;

        public ObservableCollection<IssueStatusItem> IssueStatuses { get; private set; }

        public bool IssueSearchOnlyMyIssues { get; set; }
        public ObservableCollection<IssueItem> Issues { get; private set; }

        protected IssueItem selectedIssue;
        public IssueItem SelectedIssue
        {
            get
            {
                return selectedIssue;
            }
            set
            {
                selectedIssue = value;
                trackingView.SelectIssue(selectedIssue);
            }
        }
        public ObservableCollection<ListItem> ProfileProjects { get; private set; }
        public ObservableCollection<ListItem> SortOptions { get; private set; }

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
            var statuses = new List<string>();
            foreach (IssueStatusItem status in IssueStatuses)
            {
                if (status.Selected)
                {
                    statuses.Add(status.Key);
                }
            }

            Issues = await signedJira.Search(searchString, SelectedSorting.Key, IssueSearchOnlyMyIssues, SelectedProject.Key != "-1" ? SelectedProject.Key : "", statuses, userState.UserProfile);
            this.RaisePropertyChanged("Issues");

        }

        public ListItem SelectedSorting { get; private set; }

        public ListItem SelectedProject { get; set; }


        public CurrentTrackingViewModel trackingView;
        public Sidebar()
        {
            ServicesContainer.RegisterService("sidebar", this);

        }
        public void Init()
        {
            trackingView = ServicesContainer.GetServiceTyped<CurrentTrackingViewModel>("current_tracking");

            IssueSearchOnlyMyIssues = false;
            signedJira = ServicesContainer.GetServiceTyped<JiraWrapper>("signed_jira");
            searchString = "";
            userState = ServicesContainer.GetServiceTyped<UserState>("state");

            IssueStatuses = new ObservableCollection<IssueStatusItem>();
            foreach(IssueStatusItem status in this.userState.UserProfile.Statuses.Values)
            {
                IssueStatuses.Add(status);
            }

            ProfileProjects = new ObservableCollection<ListItem>();

            ListItem Any = new ListItem();
            Any.Name = "-- any --";
            Any.Value = "-- any --";
            Any.Key = "-1";
            ProfileProjects.Add(Any);

            SelectedProject = Any;

            foreach (ListItem project in this.userState.UserProfile.Projects.Values)
            {
                ProfileProjects.Add(project);
            }

            SortOptions = new ObservableCollection<ListItem>();
            //not selected
            ListItem item = new ListItem();
            item.Name = "Last viewed, desc";
            item.Value = "Last viewed, desc";
            item.Key = "LastViewed DESC";
            SortOptions.Add(item);

            SelectedSorting = item;

             item = new ListItem();
            item.Name = "Last viewed, asc";
            item.Value = "Last viewed, asc";
            item.Key = "LastViewed ASC";
            SortOptions.Add(item);

             item = new ListItem();
            item.Name = "Created, desc";
            item.Value = "Created, desc";
            item.Key = "Created DESC";
            SortOptions.Add(item);

             item = new ListItem();
            item.Name = "Created, asc";
            item.Value = "Created, asc";
            item.Key = "Created asc";
            SortOptions.Add(item);

            item = new ListItem();
            item.Name = "Time spent, Desc";
            item.Value = "Time spent, Desc";
            item.Key = "TimeSpent desc";
            SortOptions.Add(item);

            item = new ListItem();
            item.Name = "Time spent, asc";
            item.Value = "Time spent, asc";
            item.Key = "TimeSpent asc";
            SortOptions.Add(item);

            this.RaisePropertyChanged("SortOptions");
            this.RaisePropertyChanged("SelectedSorting");
            this.RaisePropertyChanged("ProfileProjects");
            this.RaisePropertyChanged("SelectedProject");
            this.RaisePropertyChanged("IssueStatuses");

        }
    }
}
