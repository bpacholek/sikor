using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class IssueItem : ListItem
    {
        public string Status { get; set; }

        public string Project { get; set; }

        public bool AssignedToMe { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
