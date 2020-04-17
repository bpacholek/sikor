using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class IssueStatusItem : ListItem
    {
        public bool selected;
        public bool Selected {
            get => selected;
            set
            {
                selected = value;
            }
        }

    }
}
