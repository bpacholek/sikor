using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sikor.Model;
using Sikor.Services;
using Sikor.Container;

namespace Sikor.Repository
{
    [Serializable]
    abstract public class Repository<T> : StorableServiceProvider where T: IStorable
    {
        protected Dictionary<string, T> items;

        public Repository()
        {
            if (items == null)
            {
                items = new Dictionary<string, T>();
            }
        }

        public bool Has(string key)
        {
            return items.ContainsKey(key);
        }

        public T Get(string key)
        {
            return items[key];
        }

        public void DeleteByKey(string key)
        {
            items.Remove(key);
        }

        public void Add(T project)
        {
            items.Add(project.GetId(), project);
        }



    }
}
