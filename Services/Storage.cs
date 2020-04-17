using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sikor.Services
{
    public class Storage : IService
    {
        string Folder;
        public Storage(string path)
        {
            string storagePath = path + Path.DirectorySeparatorChar + "storage";
            if (Directory.Exists(storagePath))
            {
                Folder = storagePath;
            } else
            {
                Folder = Directory.CreateDirectory(storagePath).FullName;
            }
        }

        public bool Has(string key)
        {
            return File.Exists(Folder + Path.DirectorySeparatorChar + key);
        }

        public void Set(string key, object value)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Folder + Path.DirectorySeparatorChar + key, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, value);
            stream.Close();
        }

        public T Get<T>(string key)
        {
            //jeśli plik jest utwózy binary formatter
            IFormatter formatter = new BinaryFormatter();

            //otwórz plik...
            Stream stream = new FileStream(Folder + Path.DirectorySeparatorChar + key, FileMode.Open, FileAccess.Read, FileShare.None);

            //do zmiennej Uzytkownicy przypisz zdeserializowane dane. 
            //konieczne jest tutaj rzutowanie (w nawiasie) informujące program o typie danych w pamięci
            T Object = (T)formatter.Deserialize(stream);

            //zamknij plik
            stream.Close();

            return Object;
        }
    }
}
