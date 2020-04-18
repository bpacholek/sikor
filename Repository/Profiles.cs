using System;
using System.Collections.Generic;
using Sikor.Model;

namespace Sikor.Repository
{
    /**
     * <summary>
     * Repository of Profile instance.
     * </summary>
     */
    [Serializable]
    public class Profiles : Repository.Repository<Profile>
    {
        /**
         * <summary>
         * Returns a list of profiles in a id => name format.
         * </summary>
         * <returns>Dictionary of available profiles</returns>
         */
        public Dictionary<string, string> GetIdAndNamesList()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (string id in items.Keys)
            {
                list.Add(id, items[id].Name);
            }
            return list;
        }

    }
}
