using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Threading;

namespace STW_Service
{
    public class Watcher
    {
        FileSystemWatcher SW { get; set; } //source watcher
        FileSystemWatcher TW { get; set; } //target watcher
        readonly object obj = new object(); //just a mutex
        bool enabled;
        public static readonly string dearcPath, arcPath;
        static Watcher()
        {
            using (FileStream fs = new FileStream(@"D:\УЧЁБА\C#\lab2cs\STW-Service\Paths.txt", FileMode.Open, FileAccess.Read, FileShare.None))
            using (StreamReader sr = new StreamReader(fs))
            {
                sr.ReadLine(); sr.ReadLine(); //pass 2 fist strings
                arcPath = sr.ReadLine();
                dearcPath = sr.ReadLine();
            }
        }
        public Watcher()
        {
            using (FileStream fs = new FileStream(@"D:\УЧЁБА\C#\lab2cs\STW-Service\Paths.txt", FileMode.Open, FileAccess.Read, FileShare.None))
            using (StreamReader sr = new StreamReader(fs))
            {
                SW = new FileSystemWatcher(sr.ReadLine());
                TW = new FileSystemWatcher(sr.ReadLine());
                SW.Deleted += Deleted;
                TW.Deleted += Deleted;
                SW.Created += Created;
                TW.Created += Created;
                SW.Renamed += Renamed;
                TW.Renamed += Renamed;
            }
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
            using (StreamWriter writer = new StreamWriter(@"D:\УЧЁБА\C#\lab2cs\Server\Log.txt", true)) //path to LOG
            {
                writer.WriteLine(String.Format("{0} файл {1} был {2}",
                                 DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                writer.Flush();
            }
            //Thread.Sleep(1000);
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
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(Archivator.errorLog, FileMode.OpenOrCreate)))
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
                    Task.Run(() => Archivator.Archivate(e.FullPath, arcPath));
                }
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(Archivator.errorLog, FileMode.OpenOrCreate)))
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
                if (Path.GetFileNameWithoutExtension(e.FullPath).Contains("$$$") && Path.GetDirectoryName(e.FullPath) == arcPath)
                {
                    Task.Run(() =>
                    {
                        Archivator.Dearchivate(e.FullPath, dearcPath);
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
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(Archivator.errorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}
