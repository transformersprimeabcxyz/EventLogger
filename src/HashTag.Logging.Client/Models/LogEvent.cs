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
            Properties = new List<LogEventProperty>();
        }

        [JsonProperty(PropertyName = "id")]
        public Guid UUID { get; set; }

        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "eventDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime TimeStamp { get; set; }

        [JsonProperty(PropertyName = "application", NullValueHandling = NullValueHandling.Ignore)]
        public string Application { get; set; }

        [JsonProperty(PropertyName = "module", NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; set; }

        [JsonProperty(PropertyName = "environment", NullValueHandling = NullValueHandling.Ignore)]
        public string Environment { get; set; }
        
        [JsonProperty(PropertyName = "host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }

        [JsonIgnore]
        public TraceEventType EventType { get; set; }

        [JsonProperty(PropertyName = "eventType")]
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

        [JsonProperty(PropertyName = "eventTypeId")]
        public int EventTypeId
        {
            get { return (int)EventType; }
            set { EventType = (TraceEventType)value; }
        }

        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }

        [JsonProperty(PropertyName = "eventSource", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSource { get; set; }

        [JsonProperty(PropertyName = "eventCode", NullValueHandling = NullValueHandling.Ignore)]
        public string EventCode { get; set; }

        [JsonProperty(PropertyName = "eventId", NullValueHandling = NullValueHandling.Ignore)]
        public int EventId { get; set; }

        [JsonIgnore]
        public LogEventPriority Priority { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public string PriorityName
        {
            get
            {
                return Priority.ToString();
            }
            set
            {
                if (value == null)
                {
                    Priority = default(LogEventPriority);
                    return;
                }
                Priority = (LogEventPriority)Enum.Parse(typeof(LogEventPriority), value);
            }
        }

        [JsonProperty(PropertyName = "priorityId")]
        public int PriorityId
        {
            get { return (int) Priority; }
            set { Priority = (LogEventPriority) value; }
        }

        [JsonProperty(PropertyName = "exceptions", NullValueHandling = NullValueHandling.Ignore)]
        public List<LogException> Exceptions { get; set; }

        [JsonProperty(PropertyName = "properties", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<LogEventProperty> Properties { get; set; }
    }
}
