using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class ListItem
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
