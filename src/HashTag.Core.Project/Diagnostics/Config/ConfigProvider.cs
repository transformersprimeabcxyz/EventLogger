using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{

    [JsonConverter(typeof(ProvderConfigConverter))]
    [JsonObject(IsReference = true)]
    public class ConfigProvider
    {
        public ConfigProvider()
        {
            Config = new Dictionary<string, string>();
        }
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public string Type { get; set; }

        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Config { get; set; }

    }
}
