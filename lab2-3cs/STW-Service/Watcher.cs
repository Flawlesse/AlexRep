using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace STW_Service
{
    public class Watcher
    {
        FileSystemWatcher SW { get; set; } //source watcher
        FileSystemWatcher TW { get; set; } //target watcher
        readonly object obj = new object(); //just a mutex
        bool enabled;
        public static string DearcPath { get; private set; }
        public static string ArcPath { get; private set; }
        public static string Log { get; private set; }
        public static string ErrorLog { get; private set; }
        static Watcher()
        {
                var folders = Manager.GetOptions<ServerFolders>();
                ArcPath = folders.ArcPath;
                DearcPath = folders.DearcPath;
                var logs = Manager.GetOptions<Logs>();
                Log = logs.Log;
                ErrorLog = logs.ErrorLog;
        }
        public Watcher()
        {
                SW = new FileSystemWatcher(Manager.GetOptions<ServerFolders>().SourcePath);
                TW = new FileSystemWatcher(Manager.GetOptions<ServerFolders>().TargetPath);
                SW.Deleted += Deleted;
                TW.Deleted += Deleted;
                SW.Created += Created;
                TW.Created += Created;
                SW.Renamed += Renamed;
                TW.Renamed += Renamed;
                SW.IncludeSubdirectories = TW.IncludeSubdirectories = true;
        }
        public void Start()
        {
            SW.EnableRaisingEvents = TW.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(500);
            }
        }
        public void Stop()
        {
            SW.EnableRaisingEvents = TW.EnableRaisingEvents = false;
            enabled = false;
        }
        private void WriteEntry(string fileEvent, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(Log, true))
            {
                writer.WriteLine(String.Format("{0} файл {1} был {2}",
                                 DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                writer.Flush();
            }
        }
        void Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileEvent = "удален";
                string filePath = e.FullPath;
                lock (obj)
                    WriteEntry(fileEvent, filePath);
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(ErrorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
        void Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileEvent = "создан";
                string filePath = e.FullPath;
                lock (obj)
                {
                    WriteEntry(fileEvent, filePath);
                }
                if (Path.GetDirectoryName(e.FullPath) == SW.Path)
                {
                    Task.Run(() => Archivator.Archivate(e.FullPath, ArcPath));
                }
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(ErrorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
        void Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (Path.GetFileNameWithoutExtension(e.FullPath).Contains("$$$") && Path.GetDirectoryName(e.FullPath) == ArcPath)
                {
                    Task.Run(() =>
                    {
                        Archivator.Dearchivate(e.FullPath, DearcPath);
                        Thread.Sleep(100);
                        File.Delete(e.FullPath);
                    });
                }
                else
                {
                    string fileEvent = "переименован в " + e.FullPath;
                    string filePath = e.OldFullPath;
                    lock (obj)
                    {
                        WriteEntry(fileEvent, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(ErrorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}
