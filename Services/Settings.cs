using Sikor.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    [Serializable]
    public class Settings : IService
    {
        public string LastSelectedProfile { get; set; }

        public string StoragePath { get; set; }

        public void Save(string name = "settings")
        {
            var storage = ServicesContainer.GetServiceTyped<Storage>("storage");
            storage.Set(name, this);
        }

    }


}
