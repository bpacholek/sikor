using System;

namespace Sikor.Model
{
    /**
     * <summary>
     * Reperesents the current tracking process.
     * </summary>
     */
    [Serializable]
    public class Tracking : IssueOperation
    {
        /**
         * <summary>
         * Comment related to the current tracking: stored in the worklogs'
         * report.
         * </summary>
         * <value>Text, a comment related to the worklog</value>
         */
        public string Comment { get; set; }
    }
}
