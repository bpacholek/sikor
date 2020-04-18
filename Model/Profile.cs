using System;
using System.Collections.Generic;
using Sikor.Util.Security;

namespace Sikor.Model
{

    [Serializable]
    public class Profile
    {
        /**
         * <summary>
         * Uri to the Jira instance: should include http or https.
         * </summary>
         * <value>Path to Jira instance.</value>
         */
        public string Uri { get; set; }

        /**
         * <summary>
         * Username used to sign into Jira.
         * </summary>
         * <value>Jira Username</value>
         */
        public string Username { get; set; }

        /**
        * <summary>
        * Password used to sign into Jira.
        * </summary>
        * <value>Jira Password</value>
        */
        public string Password { get; set; }

        /**
         * <summary>
         * Profile name: will be used on the list of profiles.
         * </summary>
         * <value>Profile name</value>
         */
        public string Name { get; set; }

        /**
         * <summary>
         * Cached list of projects user has access to, grouped by shortcuts
         * (ticket prefixes from Jira).
         * </summary>
         * <value>Dictionary of Projects by their respective shortcuts</value>
         */
        public Dictionary<string, Project> Projects { get; set; }

        /**
         * <summary>
         * Cached list of issues user has access to, grouped by their full Jira
         * ids (with Jira project prefixes).
         * </summary>
         * <value>Dictionary of Issues by their identifiers (Jira Keys).</value>
         */
        public Dictionary<string, Issue> Issues { get; set; }

        public Dictionary<string, Status> Statuses { get; set; }

        public List<FailedStatusUpdate> FailedStatusUpdates { get; }
        public List<FailedWorklogUpdate> FailedWorklogs { get; }

        public bool IsTracking { get; set; }

        public TrackingIssueItem TrackingIssue { get; set; }

        public string TrackingComment { get; set; }

        public Profile()
        {
            Projects = new Dictionary<string, Project>();
            Issues = new Dictionary<string, Issue>();
            Statuses = new Dictionary<string, Status>();
            FailedStatusUpdates = new List<FailedStatusUpdate>();
            FailedWorklogs = new List<FailedWorklogUpdate>();
        }

        /**
         * <summary>
         * Gets the unique identifier of a Profile (which actually is just a
         * hash of the name).
         * </summary>
         * <returns>Md5 Hash</returns>
         */
        public string GetUid()
        {
            return HashGenerator.MD5(Name);
        }

    }
}
