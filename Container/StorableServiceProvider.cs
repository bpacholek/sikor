using Sikor.Services;
using System;

namespace Sikor.Container
{
    [Serializable]
    abstract public class StorableServiceProvider : ServiceProvider
    {
        [NonSerialized]
        Storage Storage;
        public override void Init()
        {
            Storage = Container.ServiceContainer.GetServiceTyped<Storage>(typeof(Storage).ToString());
        }

        public void Save()
        {
            Storage.Set(this.GetTypeString(), this);
        }
    }
}