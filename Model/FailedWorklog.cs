using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class FailedWorklog
    {
        public DateTime Created { set; get; }
        public string IssueKey { set; get; }

        public string Summary { get; set; }

        public string Comment { get; set; }

        public DateTime From { get; set; }
        public string Duration
        {
            get
            {
                var diff = (To - Created);
                return Math.Floor(diff.TotalHours).ToString().PadLeft(2, '0') + ":" + diff.Minutes.ToString().PadLeft(2, '0') + ":" + diff.Seconds.ToString().PadLeft(2, '0');
            }
        }
        public DateTime To { get; set; }
    }
}
