using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using HashTag.Reflection;
using HashTag.Text;
using System.Runtime.Serialization;
using HashTag.Collections;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using HashTag.Properties;
using System.ComponentModel.DataAnnotations;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Generic container for a log message.  All standard messages implmement these fields
    /// </summary>
    [Serializable]
    [DataContract(Namespace = CoreConfig.WcfNamespace)]
    public partial class LogEvent
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogEvent()
            : base()
        {
            TimeStamp = DateTime.Now;
            UUID = Guid.NewGuid();
            Categories = new List<string>();
        }
        /// <summary>
        /// Sets severity to Information and message text (if any)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public LogEvent(string message = null, params object[] args)
            : this()
        {
            this.Severity = TraceEventType.Information;
            if (message != null)
            {
                this.MessageText = string.Format(message, args);
            }
        }
        /// <summary>
        /// Sets Severity to Error and message text
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public LogEvent(Exception ex, string message = null, params object[] args)
            : this(message, args)
        {
            if (ex != null)
            {
                this.AddException(ex);
            }
            this.Severity = TraceEventType.Error;
        }

        /// <summary>
        /// Unique identifier for this message.  Used when comparing messages in different message sinks. (https://www.ietf.org/rfc/rfc4122.txt)
        /// </summary>
        [DataMember]
        [JsonProperty]
        [Key]
        public Guid UUID { get; set; }

        private string _title;
        /// <summary>
        /// Short text of message. Also considered "subject" or "label" depending on sending via email or via msmq respectively.  If not explicitly set, 
        /// title will attempt to build return value from embedded values
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public string Title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_title) == true)
                {
                    var msg = "";
                    if (!string.IsNullOrWhiteSpace(MessageText))
                    {
                        msg = MessageText.Left(60, "...");
                    }
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        if (Exceptions != null && Exceptions.Count > 0)
                        {
                            msg = Exceptions[0].GetBaseException.Message.Left(60, "...");
                        }
                    }
                    return msg;
                }
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        private List<LogException> _exceptions;
        /// <summary>
        /// Exception (if any) this message is referring to.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Xml.Serialization.XmlElement]
        public List<LogException> Exceptions
        {
            get
            {
                if (_exceptions == null)
                {
                    _exceptions = new List<LogException>();
                }
                return _exceptions;
            }
            set
            {
                if (_exceptions == null)
                {
                    _exceptions = new List<LogException>();
                }
                _exceptions = value;
            }
        }

        /// <summary>
        /// Add an expanded exception to list of exceptions
        /// </summary>
        /// <param name="ex"></param>
        public void AddException(Exception ex)
        {
            Exceptions.Add(new LogException(ex));
            if (this.Severity > TraceEventType.Error)
            {
                this.Severity = TraceEventType.Error;
            }
        }


        private string _loggerName;
        /// <summary>
        /// Name of logger creating this message.  Often used for identifying which part of application this message came from since logger names often are identified with application functions
        /// </summary>
        [DataMember]
        [JsonProperty]
        public string LoggerName
        {
            get
            {
                return _loggerName;
            }
            set
            {
                _loggerName = value;
            }
        }

        private string _activeEnvironment = CoreConfig.ActiveEnvironment;
        /// <summary>
        /// Returns the currently active environment the application is running in (dev, qa, production, demo, etc) (Default: .config HashTag.Application.Environment)
        /// </summary>
        [DataMember]
        [JsonProperty]
        public string ActiveEnvironment
        {
            get { return _activeEnvironment; }
            set { _activeEnvironment = value; }
        }



        /// <summary>
        /// Subsection of application (if any) (e.g. Credit Cards, Travel, etc)
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationSubKey { get; set; }

        private string _applicationKey = CoreConfig.ApplicationName;

        /// <summary>
        /// Gets the name of the application.  If not explicitly set using &lt;appSettings name="HashTag.Application.Name"...&gt;
        /// </summary>
        [DataMember]
        [JsonProperty]
        public string ApplicationKey
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationKey) == true)
                {
                    _applicationKey = CoreConfig.ApplicationName;
                }
                return _applicationKey;
            }   
            set
            {
                _applicationKey = value;
            }
        }

        private TraceEventType _logLevel = TraceEventType.Information;
        /// <summary>
        /// Severity of message being persisted (e.g. Error, Verbose) (Default: Information)
        /// </summary>
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = -70)]
        public TraceEventType Severity
        {
            get { return _logLevel; }
            set
            {

                _logLevel = value;
                priorityReset();
            }
        }

        private LogEventPriority? _priority = null;
        /// <summary>
        /// Get and Sets how important the sender of this message considers this message.  Most implementations
        /// will not explictly set this value and allow message to determine priority based on log severity.
        /// </summary>
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Order = -60)]
        public LogEventPriority Priority
        {
            get
            {

                if (_priority.HasValue == true) return _priority.Value;
                return priorityReset();

            }
            set
            {
                _priority = value;
            }
        }
        private DateTime _createDate = DateTime.Now;
        /// <summary>
        /// Date/Time when this message was created
        /// </summary>
        [DataMember]
        [JsonProperty(Order = -100)]
        public DateTime TimeStamp
        {
            get { return Utils.Date.ToSafeDate(_createDate); }
            set { _createDate = Utils.Date.ToSafeDate(value); }
        }

        /// <summary>
        /// Resets internal priority to a value that matches LogLevel of this message
        /// </summary>
        /// <returns>Messages current priority</returns>
        internal LogEventPriority priorityReset()
        {
            _priority = this.Severity.ToPriority();
            return _priority.Value;
        }


        private string _messageText = "";


        /// <summary>
        /// Returns explictly set message text or default of this class (usually ToString())
        /// </summary>
        [DataMember]
        [JsonProperty(Order = -90)]
        public string MessageText
        {
            get
            {
                if (_messageText == null)
                {
                    if (Exceptions != null && Exceptions.Count > 0)
                    {
                        return Exceptions[0].GetBaseException.Message; //similar to what Elmah does
                    }
                }
                return _messageText;
            }
            set
            {
                _messageText = value;
            }
        }

        /// <summary>
        /// Attributes about the current HTTP context.  Usually set when an Error or Higher is logged
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogHttpContext HttpContext { get; set; }

        /// <summary>
        /// Stack trace, machine name, thread identifiers and other runtime process specific contextual information
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogMachineContext MachineContext { get; set; }

        /// <summary>
        /// Identity (both thread and Http if available) and authentication status 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogUserContext UserContext { get; set; }

        /// <summary>
        /// Miscellaneous informaition caller wishes to persist to log.  (e.g. record id, invoice number, loop index, etc) ToString() is called on Reference
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Reference { get; set; }

        /// <summary>
        /// Id that correlates multiple events on a single request
        /// </summary>
        [JsonProperty]
        public Guid ActivityId
        {
            get
            {
                var currentId = Trace.CorrelationManager.ActivityId;
                if (currentId == Guid.Empty)
                {
                    currentId = Guid.NewGuid();
                    Trace.CorrelationManager.ActivityId = currentId;
                }
                return currentId;
            }
            set
            {
                Trace.CorrelationManager.ActivityId = value;
            }
        }

        private static string _machineName = Environment.MachineName;
        /// <summary>
        /// Name of host that created this message
        /// </summary>
        [JsonProperty]
        public string MachineName { get { return _machineName; } set { _machineName = value; } }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Categories { get; set; }


        /// <summary>
        /// Supplemental unstructured data for this event
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropertyBag Properties { get; set; }

        public void AddProperty(string key, string value)
        {
            if (Properties == null) Properties = new PropertyBag();
            Properties.Add(key, value);
        }

        private int? _eventid;
        /// <summary>
        /// Numeric value describing this kind of message (e.g. 1001-Empty cart, 30202-Exceeded quota)  Often used in 
        /// place of MessageCode for systems requiring numeric messages (e.g. semantic logging)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int EventId
        {
            get
            {
                if (_eventid.HasValue == false)
                {
                    var baseValue = _priority.HasValue ? ((int)_priority.Value) : (int)LogEventPriority.Normal;
                    return baseValue + (int)Severity;
                }
                return _eventid.Value;
            }
            set
            {
                _eventid = value;
            }
        }

        /// <summary>
        /// Textual (N09099, AP3933) or numeric (9932, 5321) that identifies this message.  These codes, if used, should be unique within a Source but might be unique across several sources.  Message Codes uniquely identify a particular event. Each event source can define its own numbered events 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EventCode { get; set; }


        /// <summary>
        /// Standard format for this class
        /// </summary>
        /// <returns>String value of this class</returns>
        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine("{0:yyyy-MM-dd HH:mm:ss.fff} {1}({2}) {3}", this.TimeStamp, this.Severity, this.Priority, _messageText.Left(60, "..."));
                sb.AppendLine("{1,16} {0}", indentRow(this._messageText, 17), CoreResources.MSG_Diagnostics_MessageBuilder_FullMessage);
                sb.AppendLine("{2,16} {0}({1})", this.Severity, (int)this.Severity, CoreResources.MSG_Diagnostics_MessageBuilder_Severity);
                sb.AppendLine("{2,16} {0}({1})", this.Priority, (int)this.Priority, CoreResources.MSG_Diagnostics_MessageBuilder_Priority);
                sb.AppendLine("{1,16} {0}", this.ApplicationKey, CoreResources.MSG_Diagnostics_MessageBuilder_Application);
                sb.AppendLine("{1,16} {0}", this.ActivityId, CoreResources.MSG_Diagnostics_MessageBuilder_ActivityId);
                sb.AppendLine("{1,16} {0}", this.ActiveEnvironment, CoreResources.MSG_Diagnostics_MessageBuilder_Environment);
                sb.AppendLine("{1,16} {0}", this.MachineName, CoreResources.MSG_Diagnostics_MessageBuilder_Host);
                sb.AppendLine("{1,16} {0}", this.LoggerName, CoreResources.MSG_Diagnostics_MessageBuilder_LoggerName);
                sb.AppendLine("{1,16} {0}", (Categories == null) ? "" : string.Join("|", this.Categories.ToArray()), CoreResources.MSG_Diagnostics_MessageBuilder_Categories);
                sb.AppendLine("{1,16} {0}", Reference == null ? CoreResources.MSG_Diagnostics_NullText : Reference.ToString(), CoreResources.MSG_Diagnostics_MessageBuilder_Reference);
                sb.AppendLine("{1,16} {0}", EventId, CoreResources.MSG_Diagnostics_MessageBuilder_EventId);
                sb.AppendLine("{1,16} {0}", EventCode, CoreResources.MSG_Diagnostics_MessageBuilder_MessageCode);

                if (Properties != null)
                {
                    foreach (var prop in Properties)
                    {
                        sb.AppendLine("{0,16}:{1}", prop.Key, Properties[prop.Key].ToString().Replace(Environment.NewLine, Environment.NewLine + "      "));
                    }
                }
                if (this.Exceptions != null && this.Exceptions.Count > 0)
                {
                    foreach (var exception in Exceptions)
                    {
                        sb.AppendFormat("{2, 16} {0}{1}", exception.ToString(17), Environment.NewLine, CoreResources.MSG_Diagnostics_MessageBuilder_Exception);
                    }
                }
                else
                {
                    sb.AppendFormat("{2,16} {0}{1}", "(none)", Environment.NewLine, CoreResources.MSG_Diagnostics_MessageBuilder_Exception);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine();
                sb.AppendLine(CoreResources.MSG_Diagnostics_FormattingError);
                sb.AppendLine(ex.ToString());
            }
            return sb.ToString();
        }

        protected string indentRow(string sourceString, int spaceCount)
        {
            if (string.IsNullOrEmpty(sourceString) == true) return sourceString;
            return sourceString.Replace(Environment.NewLine, Environment.NewLine + (new string(' ', spaceCount)));
        }


        /// <summary>
        /// Set uninitialized fields to their calculated values.  After this these values won't automatically change baseed on dependent properties (e.g. priority, code, event id, user)
        /// </summary>
        public void Fix()
        {
            Title = Title;
            ApplicationKey = ApplicationKey;
            Priority = Priority;
            MessageText = MessageText;
        }


    }
}
