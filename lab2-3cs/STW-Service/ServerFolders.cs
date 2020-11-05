using Newtonsoft.Json;
using System.Configuration;

namespace STW_Service
{
    class ServerFolders
    {
        [JsonProperty("Source")]
        [ConfigurationProperty("Source")]
        public string SourcePath { get; set; }
        
        [JsonProperty("Target")]
        [ConfigurationProperty("Target")]
        public string TargetPath { get; set; }
        
        [JsonProperty("Dearchivated")]
        [ConfigurationProperty("Dearchivated")]
        public string DearcPath { get; set; }
        
        [JsonProperty("Archivated")]
        [ConfigurationProperty("Archivated")]
        public string ArcPath { get; set; }
        
    }
}
