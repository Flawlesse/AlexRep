using System.Configuration;

namespace STW_Service
{
    class FolderElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)(base["name"]);
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Path
        {
            get
            {
                return (string)(base["path"]);
            }
            set
            {
                base["path"] = value;
            }
        }
    }
}
