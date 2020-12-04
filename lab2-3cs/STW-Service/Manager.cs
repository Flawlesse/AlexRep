using System;
using System.Configuration;
using System.IO;

namespace STW_Service
{
    public class Manager
    {
        IConfigurationProvider ConfigProvider { get; }
        readonly object obj = new object(); // just a mutex
        public Manager()
        {
            string filexmlpath = ConfigurationManager.AppSettings["PathToXml"];
            string filejsonpath = ConfigurationManager.AppSettings["PathToJson"];

            try
            {
                if (File.Exists(filexmlpath))
                    ConfigProvider = new XmlParser();
                else if (File.Exists(filejsonpath))
                    ConfigProvider = new JsonParser();
                if (ConfigProvider == null)
                {
                    throw new Exception("Невозможно установить поставщика конфигурации.\nВыбран поставщик по умолчанию (App.config)");
                }
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                    if (File.Exists(errorLogPath))
                    {
                        File.AppendAllText(errorLogPath, "\n"+ DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") 
                            + " " + ex.Message);
                    }
                }
            }
        }
        public T GetOptions<T>() where T : new()
        {
            T objParsed = new T();
            try
            {
                if (ConfigProvider != null)
                    objParsed = ConfigProvider.Parse<T>();
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                    if (File.Exists(errorLogPath))
                    {
                        File.AppendAllText(errorLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            return objParsed;
        }

    }
}
