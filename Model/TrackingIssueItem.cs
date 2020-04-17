using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class TrackingIssueItem : IssueItem
    {

        public string Comment { get; set; }

        public DateTime Created { get; set; }

    }
}
