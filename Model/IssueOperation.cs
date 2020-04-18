using System;

namespace Sikor.Model
{
    /**
     * <summary>
     * Represents any temporal action's entity related to an issue.
     * </summary>
     */
    [Serializable]
    abstract public class IssueOperation : Issue
    {
        /**
         * <summary>
         * Date when the operation has been commenced.
         * </summary>
         * <value>DateTime when the operation has begun</value>
         */
        public DateTime Created { set; get; }
    }
}
