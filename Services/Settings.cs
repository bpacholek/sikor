using Sikor.Container;
using System;

namespace Sikor.Model
{
    [Serializable]
    public class Settings : StorableServiceProvider
    {
        public string LastSelectedProfile { get; set; }

    }
}
