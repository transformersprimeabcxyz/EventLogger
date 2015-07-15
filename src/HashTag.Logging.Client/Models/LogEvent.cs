using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HashTag.Diagnostics.Models
{
    /// <summary>
    /// Contains all values that comprise a single log event (or 'message').  This
    /// class is passed to 3rd party connectors (e.g. NLog, log4Net, TraceSource)
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogEvent()
        {
            UUID = Guid.NewGuid();
            TimeStamp = DateTime.Now;
            Properties = new List<LogEventProperty>();
        }

        /// <summary>
        /// Identifier of this single message.  Often used when event is
        /// persisted in two different stores (splunk,db) or via different channels 
        /// (files, service bus, web service) or when event is split-persisted
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid UUID { get; set; }

        /// <summary>
        /// Human readable, textual content of message.  For exceptions it is often ex.Message
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Timestamp of host where this message was generated.  NOTE: Persistance stores
        /// should associate their own timestamp with this message to account for clock drift
        /// </summary>
        [JsonProperty(PropertyName = "eventDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Name of application generating this event (e.g. MyAccountingSystem)
        /// </summary>
        [JsonProperty(PropertyName = "application", NullValueHandling = NullValueHandling.Ignore)]
        public string Application { get; set; }

        /// <summary>
        /// Sub-application/part of the application (e.g. AccountsPayable).  Not normally
        /// used on small systems.  Often used on systems with multiple components (web, web service, windows service, console apps)
        /// </summary>
        [JsonProperty(PropertyName = "module", NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; set; }

        /// <summary>
        /// e.g. DEV, PROD, QA
        /// </summary>
        [JsonProperty(PropertyName = "environment", NullValueHandling = NullValueHandling.Ignore)]
        public string Environment { get; set; }
        
        /// <summary>
        /// NetBios name of computer where this message was generated
        /// </summary>
        [JsonProperty(PropertyName = "host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }

        /// <summary>
        /// Core event type (Verbose, Information, Warning, Error, Critical)
        /// </summary>
        [JsonIgnore]
        public TraceEventType EventType { get; set; }

        /// <summary>
        /// Textual representation of core event type 
        /// </summary>
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

        /// <summary>
        /// Numeric value of core event type
        /// </summary>
        [JsonProperty(PropertyName = "eventTypeId")]
        public int EventTypeId
        {
            get { return (int)EventType; }
            set { EventType = (TraceEventType)value; }
        }

        /// <summary>
        /// Primary identity the system was using when this event was generated
        /// </summary>
        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }

        /// <summary>
        /// Name of the logger (normally fully qualified class name)
        /// </summary>
        [JsonProperty(PropertyName = "eventSource", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSource { get; set; }

        /// <summary>
        /// Textual identifier for this message (e.g. 'B2009' = 'Account overdrawn')
        /// </summary>
        [JsonProperty(PropertyName = "eventCode", NullValueHandling = NullValueHandling.Ignore)]
        public string EventCode { get; set; }

        /// <summary>
        /// Numeric identifier of this message (e.g. 930223 = 'User is not an administrator').  Often
        /// used in queries and for integrating with ETW scenarios.  NOTE: If not explicitly set,
        /// system creates an idempotent value based on EventType and Priority
        /// </summary>
        [JsonProperty(PropertyName = "eventId", NullValueHandling = NullValueHandling.Ignore)]
        public int EventId { get; set; }

        /// <summary>
        /// The urgency event generator considers this message and functions as a request
        /// for speed and level of response.  NOTE:  If not provided
        /// the systme will assign a default priority based on EventType
        /// </summary>
        [JsonIgnore]
        public LogEventPriority Priority { get; set; }

        /// <summary>
        /// The textal representation of the urgency event generator considers this message and functions as a request
        /// for speed and level of response (High, Medium, Low).  NOTE: If not provided
        /// then system will assign a default priority based on EventType
        /// </summary>
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

        /// <summary>
        /// The numerical representation of the urgency event generator considers this message and functions as a request
        /// for speed and level of response (High, Medium, Low).  NOTE: If not provided
        /// then system will assign a default priority based on EventType
        /// </summary>
        [JsonProperty(PropertyName = "priorityId")]
        public int PriorityId
        {
            get { return (int) Priority; }
            set { Priority = (LogEventPriority) value; }
        }

        /// <summary>
        /// List of expanded exceptions associated with this message.  Usually 0..1 but may be
        /// more (e.g. recording AggregateException)
        /// </summary>
        [JsonProperty(PropertyName = "exceptions", NullValueHandling = NullValueHandling.Ignore)]
        public List<LogException> Exceptions { get; set; }

        /// <summary>
        /// Serializable 3-tuples used to pass additional information along with this message.  Used
        /// similarly to, and in place of, collection structures (e.g. IDictionary, NameValuePairs) that have difficulties with some
        /// serializers
        /// </summary>
        [JsonProperty(PropertyName = "properties", NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<LogEventProperty> Properties { get; set; }
    }
}
