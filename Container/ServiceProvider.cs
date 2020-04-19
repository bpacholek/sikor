using System;

namespace Sikor.Container
{
    [Serializable]
    abstract public class ServiceProvider : IServiceProvider
    {
        public virtual void Register()
        {
            ServiceContainer.RegisterService(this.GetType().ToString(), this);
        }

        public virtual void Init()
        {

        }

        public virtual void PostInit()
        {

        }

        public string GetTypeString()
        {
            return this.GetType().ToString();
        }
    }
}