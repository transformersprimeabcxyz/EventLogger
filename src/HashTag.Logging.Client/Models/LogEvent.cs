using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HashTag.Diagnostics.Models
{
    public class LogEvent
    {
        public LogEvent()
        {
            UUID = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }

        public Guid UUID { get; set; }

        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "eventDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime TimeStamp { get; set; }

        [JsonProperty(PropertyName = "application", NullValueHandling = NullValueHandling.Ignore)]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }
        
        public TraceEventType EventType { get; set; }

        public string EventTypeName
        {
            get
            {
                return EventType.ToString();
            }
            set
            {
                if (value == null)
                {
                    EventType = default(TraceEventType);
                    return;
                }
                EventType = (TraceEventType)Enum.Parse(typeof(TraceEventType), value);
            }
        }

        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }

        [JsonProperty(PropertyName = "eventSource", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSource { get; set; }

        [JsonProperty(PropertyName = "eventCode", NullValueHandling = NullValueHandling.Ignore)]
        public string EventCode { get; set; }

        public int EventId { get; set; }

        [JsonProperty(PropertyName = "properties", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<LogEventProperty> Properties { get; set; }


    }
}
