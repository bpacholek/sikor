using Sikor.Services;

namespace Sikor.Container
{
    abstract public class StorableServiceProvider : ServiceProvider
    {
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