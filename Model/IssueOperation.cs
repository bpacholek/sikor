using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    /**
     * <summary>
     * Represents any temporal action's entity related to an issue.
     * </summary>
     */
    [Serializable]
    abstract public class IssueOperation
    {
        /**
         * <summary>
         * Date when the operation has been commenced.
         * </summary>
         * <value>DateTime when the operation has begun</value>
         */
        public DateTime Created { set; get; }

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
        public string IssueKey { set; get; }

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
