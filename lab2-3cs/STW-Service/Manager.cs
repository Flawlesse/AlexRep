using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace STW_Service
{
    static class Manager
    {
        static readonly string configToUse;
        static Manager()
        {
            string workdir = ConfigurationManager.AppSettings.Get("WorkDir");
            if (File.Exists(Path.Combine(workdir, "config.json")))
                configToUse = Path.Combine(workdir, "config.json");
            else
                configToUse = Path.Combine(workdir, "App.config");
        }
        public static T GetOptions<T>() where T : new()
        {
            T obj = new T();
            if (Path.GetExtension(configToUse) == ".config")
            {
                if (obj is ServerFolders folders)
                {
                    ServerFoldersConfigSection serverFolders =
                        (ServerFoldersConfigSection)ConfigurationManager.GetSection("ServerFolders");

                    folders.SourcePath = serverFolders.Folders.First(x => x.Name == "Source").Path;
                    folders.TargetPath = serverFolders.Folders.First(x => x.Name == "Target").Path;
                    folders.ArcPath = serverFolders.Folders.First(x => x.Name == "Archivated").Path;
                    folders.DearcPath = serverFolders.Folders.First(x => x.Name == "Dearchivated").Path;
                }
                else if (obj is Logs logs)
                {
                    logs.ErrorLog = ConfigurationManager.AppSettings.Get("ErrorLog");
                    logs.Log = ConfigurationManager.AppSettings.Get("Log");
                }
                else if (obj is CryptoOptions crypto)
                {
                    crypto.Key = ConfigurationManager.AppSettings.Get("CryptoKey");
                    crypto.IV = ConfigurationManager.AppSettings.Get("CryptoIV");
                }
            }
            else
            {
                string json = File.ReadAllText(configToUse);
                if (obj is ServerFolders folders)
                {
                    var tmp = JsonConvert.DeserializeObject<ServerFolders>(json);
                    (folders.SourcePath, folders.TargetPath, folders.ArcPath, folders.DearcPath) =
                    (tmp.SourcePath, tmp.TargetPath, tmp.ArcPath, tmp.DearcPath);
                }
                else if (obj is Logs logs)
                {
                    var tmp = JsonConvert.DeserializeObject<Logs>(json);
                    (logs.Log, logs.ErrorLog) = (tmp.Log, tmp.ErrorLog);
                }
                else if (obj is CryptoOptions crypto)
                {
                    var tmp = JsonConvert.DeserializeObject<CryptoOptions>(json);
                    (crypto.Key, crypto.IV) = (tmp.Key, tmp.IV);
                }
            }
            return obj;
        }

        public static string Error()
        {
            return ConfigurationManager.AppSettings.Get("Error");
        }
    }
}
