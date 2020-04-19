using System;
using System.Collections.Generic;
using System.Text;
using Sikor.Services;

namespace Sikor.Container
{
    static class ServiceContainer
    {
        /**
         * <summary>
         * Stores references to shared services.
         * </summary>
         * <typeparam name="string"></typeparam>
         * <typeparam name="IServiceProvider"></typeparam>
         * <returns></returns>
         */
        private static Dictionary<string, IServiceProvider> Services = new Dictionary<string, IServiceProvider>();

        public static void RegisterService(string name, IServiceProvider service)
        {
            Services[name] = service;
        }

        public static IServiceProvider GetService(string name)
        {
            return Services[name];
        }

        public static T GetServiceTyped<T>(string name)
        {
            return (T)Services[name];
        }

        public static void Init()
        {
            foreach(IServiceProvider service in Services.Values) {
                service.Init();
            }
        }
    }
}
