using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sikor.Container;
using System;

namespace Sikor.Services
{
    /**
     * <summary>
     * Physical storage, on the harddrive, handler.
     * </summary>
     * <todo>
     * Array access interface.
     * </todo>
     */
    public class Storage : ServiceProvider
    {
        /**
         * <summary>
         * Path to the storage folder.
         * </summary>
         */
        string Folder;
        public Storage()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string storagePath = path + Path.DirectorySeparatorChar + ".sikor";
            if (Directory.Exists(storagePath))
            {
                Folder = storagePath;
            }
            else
            {
                Folder = Directory.CreateDirectory(storagePath).FullName;
            }
        }

        /**
         * <summary>
         * Checks if an entry identified by the given key exists in the storage.
         * </summary>
         * <param name="key">string, identifier of the element</param>
         * <returns>boolean, true if exists</returns>
         */
        public bool Has(string key)
        {
            return File.Exists(Folder + Path.DirectorySeparatorChar + key);
        }

        /**
         * <summary>
         * Sets the instance of the Value under the Key in the storage folder.
         * </summary>
         * <param name="key">identifier in the storage folder</param>
         * <param name="value">object to store</param>
         */
        public void Set(string key, object value)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Folder + Path.DirectorySeparatorChar + key, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, value);
            stream.Close();
        }

        /**
         * <summary>
         * Gets an instance of a object previously stored in the storage folder.
         * </summary>
         * <param name="key">identifier</param>
         * <typeparam name="T"> type of the object previosly stored int he storage</typeparam>
         * <returns>object previosly stored int he storage</returns>
         */
        public T Get<T>(string key)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Folder + Path.DirectorySeparatorChar + key, FileMode.Open, FileAccess.Read, FileShare.None);
            T Object = (T)formatter.Deserialize(stream);
            stream.Close();

            return Object;
        }
    }
}
