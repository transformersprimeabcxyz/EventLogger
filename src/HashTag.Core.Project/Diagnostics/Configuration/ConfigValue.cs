using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    [JsonConverter(typeof(ConfigValueConverter))]
    public class ConfigValue
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Default { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigKey { get; set; }


        [JsonIgnore]
        public string ResolvedValue { get; set; }

        public override string ToString()
        {
            return ResolvedValue ?? Default ?? ConfigKey;
        }


        public static implicit operator ConfigValue(int item)
        {
            return new ConfigValue()
            {
                Default = item.ToString()
            };
        }


        public static implicit operator ConfigValue(string item)
        {
            return new ConfigValue()
            {
                Default = item
            };
        }
    }

}
