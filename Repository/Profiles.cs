using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sikor.Model;
using Sikor.Services;

namespace Sikor.Repository
{
    [Serializable]
    public class Profiles : IService
    {
        protected Dictionary<string, Profile> profiles;
        public Profiles()
        {
            if (profiles == null)
            {
                profiles = new Dictionary<string, Profile>();
            }
        }


        public Dictionary<string, string> GetIdList()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (string id in profiles.Keys)
            {
                list.Add(id, profiles[id].Name);
            }
            return list;
        }

        public bool Has(string key)
        {
            return profiles.ContainsKey(key);
        }

        public Profile Get(string key)
        {
            return profiles[key];
        }

        public void DeleteByKey(string key)
        {
            profiles.Remove(key);
        }

        public void Add(Profile project)
        {
            profiles.Add(project.GetId(), project);
        }

        public void Save(string name = "profiles")
        {
            var storage = ServicesContainer.GetServiceTyped<Storage>("storage");
            storage.Set(name, this);
        }

    }
}
