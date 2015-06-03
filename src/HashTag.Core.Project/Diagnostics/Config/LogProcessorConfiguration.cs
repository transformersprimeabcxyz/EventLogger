using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    public class LogProcessorConfiguration
    {
        public LogProcessorConfiguration()
        {
            BufferConfig = new LogBufferConfiguration();
            EnvironmentKey = "HashTag.Application.Environment";
            AppNameKey = "HashTag.Application.Name";
        }

        public LogBufferConfiguration BufferConfig { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue EnvironmentKey { get; set; }

        [JsonConverter(typeof(ConfigValueConverter))]
        public ConfigValue AppNameKey { get; set; }

        [JsonConverter(typeof(FilterConverter))]
        public List<List<ConfigProvider>> PipeLine { get; set; }

        public List<ConfigProvider> Writers { get; set; }

        public List<ConfigProvider> Filters { get; set; }

        public void ValidateConfiguration()
        {

        }
    }
   

   
    
   
    
  
    
}
