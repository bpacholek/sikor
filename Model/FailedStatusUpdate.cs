using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    /**
     * <summary>
     * Represents the failed status update operation.
     * </summary>
     */
    [Serializable]
    public class FailedStatusUpdate : IssueOperation
    {
        /**
         * <summary>
         * Textual representation of a status.
         * </summary>
         * <todo>
         * Store a reference to an actual object.
         * </todo>
         * <value>Textual representation of a Jira status</value>
         */
        public string Status { set; get; }
    }
}
