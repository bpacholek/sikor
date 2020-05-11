using Sikor.Container;
using System;
using System.IO;

namespace Sikor.Services
{
    public class Logger : ServiceProvider
    {
        StreamWriter LogFile;
        public Logger()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string storagePath = path + Path.DirectorySeparatorChar + ".sikor";
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            LogFile = new StreamWriter(storagePath + Path.DirectorySeparatorChar + "lastrun.log");
        }

        public void Log(Object source, string location, string text)
        {
            LogFile.WriteLine("[" + DateTime.Now.ToString() + "] " + source.GetType().ToString() + ":" + location + ">" + text);
            LogFile.Flush();
        }
        ~Logger()
        {
            LogFile.Close();
        }
    }
}