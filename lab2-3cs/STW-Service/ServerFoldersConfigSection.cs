using System.Configuration;

namespace STW_Service
{
    class ServerFoldersConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Folders")]
        public FoldersCollection Folders
        {
            get
            {
                return ((FoldersCollection)(base["Folders"]));
            }
        }
    }
}
