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

        /**
         * <summary>
         * Gets the duration, the difference between To and From dates in format
         * [total hours]:[minutes]:[seconds]. Values are padded to minimal length
         * of two with zeroes.
         * </summary>
         * <value>Textual representation of the interval between To and From</value>
         */
        public string Duration
        {
            get
            {
                var diff = (To - Created);
                return Math.Floor(diff.TotalHours).ToString().PadLeft(2, '0') + ":" + diff.Minutes.ToString().PadLeft(2, '0') + ":" + diff.Seconds.ToString().PadLeft(2, '0');
            }
        }

        /**
         * <summary>
         * Time (full date) when the tracking of an issue has been marked as
         * finished.
         * </summary>
         * <value>DateTime of the moment when tracking has finished</value>
         */
        public DateTime To { get; set; }
    }
}
