using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class FailedStatusUpdateOperation
    {
        public DateTime Created { set; get; }
        public string IssueKey { set; get; }

        public string Summary { get; set; }
        public string Status { set; get; }
    }
}
