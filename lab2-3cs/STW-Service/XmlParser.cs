﻿using System;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Runtime.Remoting;
using System.Configuration;
using System.IO;

namespace STW_Service
{
    class XmlParser : IConfigurationProvider
    {
        private bool Valid { get; set; } = false;
        private bool ValidationNeeded { get; } = false;
        public XmlParser() 
        {
            try
            {
                ValidationNeeded = Convert.ToBoolean(ConfigurationManager.AppSettings["Validate"]);
            }
            catch (Exception)
            {
                string validationLogPath = ConfigurationManager.AppSettings["ValidationLogPath"];
                if (File.Exists(validationLogPath))
                {
                    File.AppendAllText(validationLogPath, "\nНе ясно, требуется ли валидация. По умолчанию валидация не требуется, " +
                        "возможен неполный парсинг (некоторые поля будут заданы по умолчанию).\n");
                }
            }
        }
        public T Parse<T>() where T : new()
        {
            Type typeNeeded = typeof(T);
            
            if (ValidationNeeded && !Validate())
            {
                string validationLogPath = ConfigurationManager.AppSettings["ValidationLogPath"];
                if (File.Exists(validationLogPath))
                {
                    File.AppendAllText(validationLogPath, "Ошибка валидации XML.\nВозвращён экземпляр объекта класса " +
                        $"{typeNeeded.FullName} по умолчанию.\n");
                }
                return new T();
            }
            if (!ValidationNeeded)
            {
                string validationLogPath = ConfigurationManager.AppSettings["ValidationLogPath"];
                if (File.Exists(validationLogPath))
                {
                    File.AppendAllText(validationLogPath, "Валидация XML не требуется.\nВозможен " +
                        "неполный парсинг (некоторые поля будут заданы по умолчанию).\n");
                }
            }

            ParsableAttribute cattr = typeNeeded.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
            XmlDocument xDoc = new XmlDocument();
            string xmlpath = ConfigurationManager.AppSettings["PathToXml"];
            xDoc.Load(xmlpath);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            if (cattr != null)
            {
                try
                {
                    return GetObjectFromRoot<T>(xRoot, cattr.Alias, typeNeeded);
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
            else
            {
                try
                {
                    return GetObjectFromRoot<T>(xRoot, typeNeeded.Name, typeNeeded);
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
        }
        private T GetObjectFromRoot<T>(XmlElement xRoot, string name, Type typeNeeded)
        {
            XmlNode xnode = null;
            foreach (XmlNode node in xRoot)
            {
                if (node.Name == name)
                {
                    xnode = node;
                    break;
                }
            }
            if (xnode == null)
            {
                throw new Exception($"Element {name} not found in XML.");
            }
            // вызов парсера узла
            MethodInfo nodeGenericParser = typeof(XmlParser)
                .GetMethod("ParseNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .MakeGenericMethod(typeNeeded);
            object[] args = { xnode };
            T obj = (T)nodeGenericParser.Invoke(this, args); // call T ParseNode<T>(selected node)
            return obj;
        }
        private T ParseNode<T>(XmlNode nodeSelected) where T : new()
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
            object complexobj = handle.Unwrap(); // объект типа, полученного из тега, созданный в рантайме

            if (nodeSelected.Attributes.Count > 0) // получаем атрибуты тега
            {
                foreach (XmlAttribute xattr in nodeSelected.Attributes)
                {
                    if (xattr != null)
                    {
                        SetPrimitiveXml<XmlAttribute>(complexobj, xattr);
                    }
                }
            }
            // обходим все дочерние узлы
            if (nodeSelected.HasChildNodes)
            {
                foreach (XmlNode xxnode in nodeSelected.ChildNodes)
                {
                    if (xxnode != null)
                    {   // просто текст посреди тега
                        if (xxnode.ChildNodes.Count == 1 && xxnode.FirstChild.NodeType == XmlNodeType.Text && xxnode.Attributes.Count == 0)
                        {
                            SetPrimitiveXml<XmlNode>(complexobj, xxnode);
                        }
                        else
                        {
                            // another complex property INSIDE a complex object
                            SetEmbeddedObject(complexobj, xxnode);
                        }
                    }
                }
            }
            return (T)complexobj; // вернуть объект после парсинга
        }
        private void SetPrimitiveXml<T>(object complexobj, T partOfTag) // T может быть XmlAttribute и XmlNode
        {
            Type t = typeof(T);
            try
            {
                if (t == typeof(XmlAttribute))
                {
                    XmlAttribute xattr = partOfTag as XmlAttribute;
                    PropertyInfo[] props = complexobj.GetType()
                                .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo pi in props)
                    {
                        ParsableAttribute cattr = pi.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                        if (cattr != null && cattr.Alias == xattr.Name)
                        {
                            object val = Convert.ChangeType(xattr.Value, pi.PropertyType);
                            pi.SetValue(complexobj, val);
                        }
                    }
                }
                else if (t == typeof(XmlNode))
                {
                    XmlNode xnode = partOfTag as XmlNode;
                    PropertyInfo[] props = complexobj.GetType()
                                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo pi in props)
                    {
                        ParsableAttribute cattr = pi.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                        if ((cattr != null && cattr.Alias == xnode.Name) || pi.Name == xnode.Name)
                        {
                            object val = Convert.ChangeType(xnode.InnerText.Trim(), pi.PropertyType);
                            pi.SetValue(complexobj, val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                if (File.Exists(errorLogPath))
                {
                    File.AppendAllText(errorLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        private void SetEmbeddedObject(object complexobj, XmlNode childnode)
        {
            try
            {
                PropertyInfo[] props = complexobj.GetType()
                                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo pi in props)
                {
                    ParsableAttribute cattr = pi.GetCustomAttribute(typeof(ParsableAttribute)) as ParsableAttribute;
                    if ((cattr != null && cattr.Alias == childnode.Name) || pi.Name == childnode.Name)
                    {
                        // извлекаем нужный нам тип
                        Type typeNeeded = pi.PropertyType;
                        MethodInfo nodeGenericParser = typeof(XmlParser)
                            .GetMethod("ParseNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                            .MakeGenericMethod(typeNeeded);
                        object[] args = { childnode };
                        object val = nodeGenericParser.Invoke(this, args);
                        if (val.GetType() == typeNeeded)
                        {
                            pi.SetValue(complexobj, val);
                        }
                        else
                        {
                            throw new Exception($"Неподходящий тип:\n\tЛевый операнд: {pi.PropertyType}" +
                                $"\n\tПравый операнд: {val.GetType()}\nБудет присвоено значение по умолчанию.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogPath"];
                if (File.Exists(errorLogPath))
                {
                    File.AppendAllText(errorLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        private bool Validate()
        {
            // Create the XmlSchemaSet class.
            try
            {
                XmlSchemaSet xsdSchema = new XmlSchemaSet();

                // Add the schema to the collection.
                string targetNamespace = ConfigurationManager.AppSettings["targetNS"];
                string schemaURI = ConfigurationManager.AppSettings["XSDPath"];
                if (targetNamespace == null || schemaURI == null)
                {
                    throw new Exception("Не найдена схема валидации либо сам файл XML.\nВыбран поставщик конфигурации по" +
                        " умолчанию (App.config)");
                }
                xsdSchema.Add(targetNamespace, schemaURI);

                // Set the validation settings.
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = xsdSchema;
                settings.ValidationEventHandler += ValidationCallBack;

                // Create the XmlReader object.
                string pathToXml = ConfigurationManager.AppSettings["PathToXml"];
                XmlReader reader = XmlReader.Create(pathToXml, settings);

                // Parse the file.
                while (reader.Read()) ;
                Valid = true;
            }
            catch (Exception ex)
            {
                string validationLogPath = ConfigurationManager.AppSettings["ValidationLogPath"];
                if (File.Exists(validationLogPath))
                {
                    File.AppendAllText(validationLogPath, "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
            return Valid;
        }
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                Valid = false;
                string validationLogPath = ConfigurationManager.AppSettings["ValidationLogPath"];
                if (File.Exists(validationLogPath))
                {
                    File.AppendAllText(validationLogPath, "\n" + e.Message);
                }
            }
        }
    }
}
