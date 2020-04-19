using System;
using System.Collections.Generic;
using Sikor.Util.Security;
using Sikor.Repository;

namespace Sikor.Model
{
    /**
     * <summary>
     * Represents a Profile which store access details, cached issues, stored
     * for later failed worklogs or status update and details about current
     * tracking process.
     * </summary>
     */
    [Serializable]
    public class Profile : IStorable
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
         * Cached list of issues user has access to, identified by their full Jira
         * ids (with Jira project prefixes).
         * </summary>
         * <value>Dictionary of Issues by their identifiers (Jira Keys).</value>
         */
        public Dictionary<string, Issue> Issues { get; set; }

        /**
         * <summary>
         * Cached list of statuses user has access to, identified by their Jira
         * ids (usually numerical).
         * </summary>
         * <todo>
         * Verify if key is always numerical
         * </todo>
         * <value>Dictionary of Statuses by their Jira keys (usually numerical)</value>
         */
        public Dictionary<string, Status> Statuses { get; set; }

        /**
         * <summary>
         * List of statuses which were not saved due to some issues: allows to
         * save them again after solving the problem (network, credentials etc.)
         * </summary>
         * <value>List of FailedStatusUpdate</value>
         */
        public List<FailedStatusUpdate> FailedStatusUpdates { get; }

        /**
         * <summary>
         * List of failed worklog updates; whenever a network error etc. error
         * occured then entry is stored for later.
         * </summary>
         * <value></value>
         */
        public List<Tracking> FailedWorklogs { get; }

        /**
         * <summary>
         * Answers the questions if Profile is currently in tracking state (has
         * a tracking issue)
         * </summary>
         * <value>Boolean, true if profile is tracking time for an issue</value>
         */
        public bool IsTracking
        {
            get
            {
                return CurrentTracking != null;
            }
        }

        /**
         * <summary>
         * Current time-tracking process or null.
         * </summary>
         * <value>Tracking instance with comment and time</value>
         */
        public Tracking CurrentTracking { get; set; }

        public Issue SelectedIssue { get; set; }

        public Profile()
        {
            Projects = new Dictionary<string, Project>();
            Issues = new Dictionary<string, Issue>();
            Statuses = new Dictionary<string, Status>();
            FailedStatusUpdates = new List<FailedStatusUpdate>();
            FailedWorklogs = new List<Tracking>();
        }

        /**
         * <summary>
         * Gets the unique identifier of a Profile (which actually is just a
         * hash of the name).
         * </summary>
         * <returns>Md5 Hash</returns>
         */
        public string GetId()
        {
            return HashGenerator.MD5(Name);
        }

    }
}
