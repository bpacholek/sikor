using ReactiveUI;
using Sikor.Services;

namespace Sikor.Container
{
    abstract public class ReactiveViewServiceProvider : ReactiveObject, IServiceProvider
    {
        public AppState AppState;
        public virtual void Register()
        {
            string className = this.GetType().ToString();

            ServiceContainer.RegisterService(this.GetType().ToString(), this);
        }

        public virtual void Init()
        {
            string appState = typeof(Sikor.Services.AppState).ToString();
            AppState = ServiceContainer.GetServiceTyped<AppState>(appState);
        }

        public ReactiveViewServiceProvider()
        {
            Register();
            Init();
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