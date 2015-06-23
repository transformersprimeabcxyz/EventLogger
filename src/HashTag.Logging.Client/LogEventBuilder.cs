using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using HashTag.Collections;
using HashTag.Diagnostics.Models;
using Newtonsoft.Json;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Facade to set values on the underlying message to be persisted to log
    /// </summary>
    public class LogEventBuilder
    {
        private ILogEventProcessor _defaultProcessor;

        internal LogEventBuilder(ILogEventProcessor processor)
        {
            _defaultProcessor = processor;
        }

        private string _messageText;
        /// <summary>
        /// Persist all built up properties to persistent store.  This MUST be called for peristance to take place and MUST be last method in fluent chain
        /// </summary>
        /// <param name="message">Message text of log message.  May include any standard string.format paramters</param>
        /// <param name="args">Any argument to supply to <paramref name="message"/></param>
        public LogEvent Write(string message, params object[] args)
        {
            _messageText = string.Format(message, args);
            var evt = ConvertToEvent(this);
            if (_defaultProcessor != null)
            {
                _defaultProcessor.Submit(evt);
            }
            return evt;
        }

        /// <summary>
        /// Persist an object to log.  Uses <paramref name="messageData"/>.ToString() to serialize the message.   This MUST be called for peristance to take place and MUST be last method in fluent chain
        /// </summary>
        /// <param name="messageData">Data to be persisted to log.</param>
        public LogEvent Write(object messageData = null)
        {
            var evt = ConvertToEvent(this);            
            if (messageData != null)
            {
                if (messageData is Exception)
                {
                    return Catch(messageData as Exception).Write();
                }
                Write(messageData.ToString());
            }
            else
            {
                if (_defaultProcessor != null)
                {
                    _defaultProcessor.Submit(evt);
                }
            }
            return evt;
        }

        public LogEvent Write(Exception ex, string message=null, params object[] args)
        {
            Catch(ex);
            if (string.IsNullOrWhiteSpace(message))
            {
                return Write((object)null);
            }
            else
            {
                return Write(message, args);
            }
        }


        private int _eventId;
        /// <summary>
        /// Identifier of this message (e.g. '101', '34334').  Used in IT departments where there is a preference to log by number instead of by category and/or name.
        /// If not provided system will use a combination of Severity+Priority
        /// </summary>
        /// <param name="eventId">User defined numerical id of message</param>
        /// <returns></returns>
        public LogEventBuilder WithId(int eventId)
        {
            _eventId = eventId;
            return this;
        }

        private string _eventCode;
        /// <summary>
        /// Textual (N09099, AP3933) or numeric (9932, 5321) that identifies this message.  These codes, if used, should be unique within a Source but might be unique across several sources.  Message Codes uniquely identify a particular event. Each event source can define its own numbered events 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public LogEventBuilder WithCode(string code, params object[] args)
        {
            _eventCode = string.Format(code, args);
            return this;
        }

        private string _appSubKey;
        /// <summary>
        /// Set sub application key.  The application key is set in HashTag.Application.Name .config key
        /// </summary>
        /// <param name="moduleName">Name of sub-part of application</param>
        /// <returns></returns>
        public LogEventBuilder ForModule(string moduleName)
        {
            _appSubKey = moduleName;
            return this;
        }


        private PropertyBag _properties;
        /// <summary>
        /// Add a new value to the KeyValue pair collection for this message
        /// </summary>
        /// <param name="key">Name for item.</param>
        /// <param name="value">Value to add.  Uses <paramref name="value"/>.ToString() to store value</param>
        /// <returns></returns>
        public LogEventBuilder Collect(string key, object value) // extended properties
        {
            if (_properties == null)
            {
                _properties = new PropertyBag();
            }
            _properties.Add(key, value.ToString());
            return this;
        }

        private List<LogException> _exceptions;

        /// <summary>
        /// Store detailed exception information into message.  NOTE: If message text is not supplied and
        /// there is an <paramref name="ex"/> stored, then this message will use <paramref name="ex"/>.Message
        /// as the message text.  This makes logging generic exceptions very easy to do.
        /// </summary>
        /// <param name="ex">Hydrated exception to store</param>
        /// <returns></returns>
        public LogEventBuilder Catch(Exception ex)
        {
            if (_exceptions == null)
            {
                _exceptions = new List<LogException>();
            }
            _exceptions.Add(new LogException(ex));
           
            return this;
        }


        private LogHttpContext _httpContext;
        /// <summary>
        /// Extract specific HTTP information and store it in message's HttpContext collection.
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <param name="flags">Determines which information to collect from context</param>
        /// <returns></returns>
        public LogEventBuilder CaptureHttp(HttpCaptureFlags flags)
        {
            _httpContext = new LogHttpContext(HttpContext.Current, flags);
            return this;
        }

        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public LogEventBuilder Reference(string reference, params object[] args)
        {
            _reference = (object)string.Format(reference, args);
            return this;
        }

        private object _reference;
        /// <summary>
        /// Convenience.  Set the Title of message to <paramref name="reference"/>.  Value is <paramref name="reference"/>.ToString()
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder Reference(object reference)
        {
            _reference = (object)reference.ToString();
            return this;
        }

        /// <summary>
        /// Extract HTTP information and store it in message's HttpContext collection (Default: Url and Form variables).
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureHttp()
        {
            return CaptureHttp(HttpCaptureFlags.All);
        }

        private LogMachineContext _machineContext;

        /// <summary>
        /// Retrieve operating system and network settings (e.g. thread id, host name, IP addresses, process identifiers)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureMachineContext()
        {
            _machineContext = new LogMachineContext();
            return this;
        }

        private LogUserContext _userContext;
        /// <summary>
        /// Retrieve HTTP and Thread identity (names, etc)
        /// WARNING: This is an extemely heavy operation and should only be done in extreme cases (e.g. logging exceptions)
        /// </summary>
        /// <returns></returns>
        public LogEventBuilder CaptureIdentity()
        {
            _userContext = new LogUserContext();
            return this;
        }

        /// <summary>
        /// Hide ToString() from Intellisense in client application
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Hide Equals() from Intellisense in client application
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Hide GetHashCode from Intellisense in client application
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static LogEvent ConvertToEvent(LogEventBuilder evtBuilder)
        {
            var retVal = new LogEvent();
            retVal.Application = evtBuilder.ApplicationKey;
            retVal.EventDate = evtBuilder.TimeStamp;
            retVal.EventSource = evtBuilder.LoggerName;
            retVal.EventType = evtBuilder.Severity;
            retVal.EventTypeName = evtBuilder.Severity.ToString();
            retVal.Host = evtBuilder.MachineName;
            retVal.Message = evtBuilder.MessageText;
            retVal.User = evtBuilder.UserIdentity;
            if (evtBuilder.UserContext != null)
            {
                retVal.User = evtBuilder.UserContext.DefaultUser;
            }
            retVal.UUID = evtBuilder.UUID;

            if (evtBuilder.Exceptions != null && evtBuilder.Exceptions.Count > 0)
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Exceptions", Value = JsonConvert.SerializeObject(evtBuilder.Exceptions, Formatting.Indented) });
            }
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Environment", Value = evtBuilder.ActiveEnvironment });
            if (!string.IsNullOrWhiteSpace(evtBuilder.ApplicationSubKey))
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "SubKey", Value = evtBuilder.ApplicationSubKey });
            }
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Priority", Value = ((int)evtBuilder.Priority).ToString() });
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "PriorityName", Value = evtBuilder.Priority.ToString() });
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "ActivityId", Value = evtBuilder.ActivityId.ToString() });
            if (evtBuilder.Reference != null)
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Reference", Value = JsonConvert.SerializeObject(evtBuilder.Reference, Formatting.Indented) });
            }
        
            if (evtBuilder.Properties != null && evtBuilder.Properties.Count > 0)
            {
                foreach (var prop in evtBuilder.Properties)
                {
                    retVal.Properties.Add(new LogEventProperty() { Group = "Properties", Name = prop.Key, Value = prop.Value });
                }
            }

            if (evtBuilder.HttpContext != null)
            {
                convertToEvent(retVal.Properties, evtBuilder.HttpContext);
            }
            if (evtBuilder.MachineContext != null)
            {
                convertToEvent(retVal.Properties, evtBuilder.MachineContext);
            }
            if (evtBuilder.UserContext != null)
            {
                convertToEvent(retVal.Properties, evtBuilder.UserContext);
            }

            retVal.Properties.ForEach(p =>
            {
                p.Event = retVal;
                p.EventUUID = retVal.UUID;
            });
            return retVal;
        }

        private static void convertToEvent(List<LogEventProperty> list, LogUserContext logUserContext)
        {
            if (!string.IsNullOrWhiteSpace(logUserContext.AppDomainIdentity))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "AppDomainIdentity", Value = logUserContext.AppDomainIdentity });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.DefaultUser))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "DefaultUser", Value = logUserContext.DefaultUser });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.EnvUserName))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "EnvUserName", Value = logUserContext.EnvUserName });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.HttpUser))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "HttpUser", Value = logUserContext.HttpUser });
            }
            if (logUserContext.IsInteractive.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "IsInteractive", Value = logUserContext.IsInteractive.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logUserContext.ThreadPrincipal))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "ThreadPrincipal", Value = logUserContext.ThreadPrincipal });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.UserDomain))
            {
                list.Add(new LogEventProperty() { Group = "UserContext", Name = "UserDomain", Value = logUserContext.UserDomain });
            }
        }
        private const int BytesInMB = (1024 * 1024 * 1024);
        private static void convertToEvent(List<LogEventProperty> list, LogMachineContext logMachineContext)
        {
            if (!string.IsNullOrWhiteSpace(logMachineContext.AppDomainName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "AppDomainName", Value = logMachineContext.AppDomainName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.AppFolder))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "AppFolder", Value = logMachineContext.AppFolder });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ClassName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "ClassName", Value = logMachineContext.ClassName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.CommandLine))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "CommandLine", Value = logMachineContext.CommandLine });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAppIdentity))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainAppIdentity", Value = logMachineContext.DomainAppIdentity });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAppName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainAppName", Value = logMachineContext.DomainAppName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAssmName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainAssmName", Value = logMachineContext.DomainAssmName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAssmVersion))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainAssmVersion", Value = logMachineContext.DomainAssmVersion });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainConfigFile))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainConfigFile", Value = logMachineContext.DomainConfigFile });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainCtxIdentity))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainCtxIdentity", Value = logMachineContext.DomainCtxIdentity });
            }
            if (logMachineContext.DomainId.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "DomainId", Value = logMachineContext.DomainId.Value.ToString() });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.HostName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "HostName", Value = logMachineContext.HostName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.IPAddressList))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "IPAddressList", Value = logMachineContext.IPAddressList });
            }

            if (logMachineContext.Is64BitOperatingSystem.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "Is64BitOperatingSystem", Value = logMachineContext.Is64BitOperatingSystem.Value.ToString() });
            }
            if (logMachineContext.Is64BitProcess.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "Is64BitProcess", Value = logMachineContext.Is64BitProcess.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logMachineContext.ManagedThreadName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "ManagedThreadName", Value = logMachineContext.ManagedThreadName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.MethodName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "MethodName", Value = logMachineContext.MethodName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.OsVersion))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "OsVersion", Value = logMachineContext.OsVersion });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ProcessId))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "ProcessId", Value = logMachineContext.ProcessId });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ProcessName))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "ProcessName", Value = logMachineContext.ProcessName });
            }

            if (logMachineContext.ProcessorCount.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "ProcessorCount", Value = logMachineContext.ProcessorCount.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logMachineContext.StackTrace))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "StackTrace", Value = logMachineContext.StackTrace });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.Win32ThreadId))
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "Win32ThreadId", Value = logMachineContext.Win32ThreadId });
            }
            if (logMachineContext.WorkingMemoryBytes.HasValue)
            {
                list.Add(new LogEventProperty() { Group = "MachineContext", Name = "WorkingMemoryMB", Value = string.Format("{0:0.000}", logMachineContext.WorkingMemoryBytes.Value / BytesInMB) });
            }
        }

        private static void convertToEvent(List<LogEventProperty> list, LogHttpContext logHttpContext)
        {
            convertToEvent(list, logHttpContext.ServerVariables, "HttpContext.Server");
            convertToEvent(list, logHttpContext.Cookies, "HttpContext.Cookies");
            convertToEvent(list, logHttpContext.Form, "HttpContext.Form");
            convertToEvent(list, logHttpContext.Headers, "HttpContext.Headers");
            convertToEvent(list, logHttpContext.QueryString, "HttpContext.QueryString");
        }
        private static void convertToEvent(List<LogEventProperty> list, PropertyBag props, string groupName)
        {
            if (props == null || props.Count == 0) return;

            list.AddRange(props.Select(prop => new LogEventProperty() { Group = groupName, Name = prop.Key, Value = prop.Value }));
        }
    }
}
