namespace Sikor.Container
{
    abstract public class ServiceProvider : IServiceProvider
    {
        public virtual void Register()
        {
            ServiceContainer.RegisterService(this.GetType().ToString(), this);
        }

        public virtual void Init()
        {

        }

        public string GetTypeString()
        {
            return this.GetType().ToString();
        }
    }
}