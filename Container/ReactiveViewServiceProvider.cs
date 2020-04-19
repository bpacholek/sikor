using ReactiveUI;
using Sikor.Services;

namespace Sikor.Container
{
    abstract public class ReactiveViewServiceProvider : ReactiveObject, IServiceProvider
    {
        public AppState AppState;
        public virtual void Register()
        {
            ServiceContainer.RegisterService(this.GetType().ToString(), this);
        }

        public virtual void Init()
        {
            AppState = ServiceContainer.GetServiceTyped<AppState>(typeof(AppState).GetType().ToString());
        }

        public ReactiveViewServiceProvider()
        {
            Register();
        }

        public string GetTypeString()
        {
            return this.GetType().ToString();
        }
    }
}