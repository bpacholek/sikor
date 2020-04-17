using System;
using System.Collections.Generic;
using System.Text;
using Sikor.Services;

namespace Sikor
{
    static class ServicesContainer
    {
        private static Dictionary<string, IService> Services = new Dictionary<string, IService>();

        public static void RegisterService(string name, IService service)
        {
            Services[name] = service;
        }

        public static IService GetService(string name)
        {
            return Services[name];
        }

        public static T GetServiceTyped<T>(string name)
        {
            return (T)Services[name];
        }
    }
}
