using System;

namespace Sikor.Model
{
    [Serializable]
    public class Issue : ListableItem
    {
        /**
         * <summary>
         * Textual representation of a status.
         * </summary>
         * <todo>
         * We do not store any actual reference to an object as so far it seems
         * useless: search within Jira is executed via textual representations,
         * yet for purists' sanity we should actually reference here an object.
         * </todo>
         * <value>string, textual status</value>
         */
        public string Status { get; set; }

        /**
         * <summary>
         * Stores the textual representation of the project, identified by the
         * project's key (the short, usually capital-case description of a
         * particular project from Jira).
         * </summary>
         * <todo>
         * The same case as with Status: at some point may be sane to reference
         * actual objects.
         * </todo>
         * <value></value>
         */
        public string ProjectKey { get; set; }

        /**
         * <summary>
         * Informs if issue is assigned to current user.
         * </summary>
         * <todo>
         * make more generic
         * </todo>
         * <value>true if issue belongs to current user</value>
         */
        public bool AssignedToMe { get; set; }

        /**
         * <summary>
         * Returns the textual represntation of the issue (its summary).
         * </summary>
         * <returns>name (summary)</returns>
         */
        public override string ToString()
        {
            return Value;
        }

        /**
         * <summary>
         * Jira Key of an issue (ABC-001).
         * </summary>
         * <todo>
         * May become obsolete in the future in favor of an actual reference to
         * an issue. Stored as text due to the possibility of losing sync with
         * Jira.
         * </todo>
         * <value>Issue's Jira Key</value>
         */
        public string IssueKey => Key;

        /**
         * <summary>
         * Issue's summary (its name).
         * </summary>
         * <todo>
         * Will become obsolete when we cross-reference actual issue here: stored
         * for cases when an issue is still tracked but no longer available in
         * Jira.
         * </todo>
         * <value>Issues summary (name)</value>
         */
        public string Summary { get; set; }

    }
}
