using System;
using Newtonsoft.Json;

namespace HashTag.Diagnostics.Models
{

    public class LogEventProperty
    {
        public LogEventProperty()
        {
            UUID = Guid.NewGuid();
        }

        public Guid UUID { get; set; }

        public Guid EventUUID { get; set; }

        [JsonProperty(PropertyName = "group", NullValueHandling = NullValueHandling.Ignore)]
        public string Group { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]

        public string Value { get; set; }
    }

   
}
