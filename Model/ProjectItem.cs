using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class ProjectItem : ListItem
    {
        public string Shortcut { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
