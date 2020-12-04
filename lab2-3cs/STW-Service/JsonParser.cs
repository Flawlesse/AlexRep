using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Configuration;
using System.Text.Json;
using System.IO;

namespace STW_Service
{
    class JsonParser: IConfigurationProvider
    {
        public JsonParser() { }
        public T Parse<T>() where T : new()
        {
            T obj;
            Type typeNeeded = typeof(T);
            string jsonpath = ConfigurationManager.AppSettings["PathToJson"];
            string jsonText = File.ReadAllText(jsonpath);

            using (JsonDocument jDoc = JsonDocument.Parse(jsonText))
            {
                // исследуем корень
                JsonElement root = jDoc.RootElement;
                ParsableAttribute cattr = typeNeeded.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                JsonElement el;
                if (cattr != null)
                    el = root.GetProperty(cattr.Alias);
                else
                    el = root.GetProperty(typeNeeded.Name);

                if (el.ValueKind != JsonValueKind.Null && el.ValueKind != JsonValueKind.Undefined)
                {
                    // создаём и вызываем T ParseNode<T>(selected node)
                    MethodInfo nodeGenericParser = typeof(JsonParser)
                        .GetMethod("ParseNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .MakeGenericMethod(typeNeeded);
                    object[] args = { el };
                    try
                    {
                        obj = (T)nodeGenericParser.Invoke(this, args); 
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                        if (File.Exists(errorLogPath))
                        {
                            File.AppendAllText(errorLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                        return new T();
                    }
                }
                return new T();
            }
        }
        private T ParseNode<T>(JsonElement nodeSelected) where T : new()
        {
            Type t = typeof(T);
            string objname = t.FullName;
            ObjectHandle handle;
            try
            {
                handle = Activator.CreateInstance(null, objname); // из текущей сборки
            }
            catch (Exception ex)
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                if (File.Exists(errorLogPath))
                {
                    File.AppendAllText(errorLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                }
                return new T();
            }
            object complexobj = handle.Unwrap(); 
            foreach (JsonProperty el in nodeSelected.EnumerateObject())
            {
                if (el.Value.ValueKind != JsonValueKind.Null && el.Value.ValueKind != JsonValueKind.Undefined)
                {
                    // свойство объекта complexobj является сложным пользовательским типом
                    if (el.Value.ValueKind == JsonValueKind.Object)
                    {
                        PropertyInfo[] props = complexobj.GetType()
                            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo pi in props)
                        {
                            ParsableAttribute cattr = pi.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                            if ((cattr != null && cattr.Alias == el.Name) || pi.Name == el.Name)
                            {
                                // извлекаем нужный нам тип
                                Type typeNeeded = pi.PropertyType;
                                MethodInfo nodeGenericParser = typeof(JsonParser)
                                    .GetMethod("ParseNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                    .MakeGenericMethod(typeNeeded);
                                object[] args = { el.Value };
                                object val = nodeGenericParser.Invoke(this, args);
                                if (val.GetType() == typeNeeded)
                                    pi.SetValue(complexobj, val);
                            }
                        }
                    }
                    // свойство объекта complexobj является примитивом
                    else
                    {
                        PropertyInfo[] props = complexobj.GetType()
                            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo pi in props)
                        {
                            ParsableAttribute cattr = pi.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                            if ((cattr != null && cattr.Alias == el.Name) || pi.Name == el.Name)
                            {
                                object val = Convert.ChangeType(el.Value.GetString(), pi.PropertyType);
                                pi.SetValue(complexobj, val);
                            }
                        }
                    }
                }
            }
            return (T)complexobj;
        }
    }
}
