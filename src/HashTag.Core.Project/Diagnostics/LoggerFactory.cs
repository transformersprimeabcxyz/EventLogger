using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HashTag.Diagnostics.Models;
using Newtonsoft.Json;

namespace HashTag.Diagnostics
{
    public class LoggerFactory 
    {
        private static ConcurrentDictionary<string, Logger> _registeredLogs = new ConcurrentDictionary<string, Logger>();
        public static string _defaultLogName { get; set; }

        private static ILogEventProcessor _eventProcessor = new LogEventProcessor();


        /// <summary>
        /// Create a new log instance with application name.  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:Vital)
        /// </summary>
        /// <returns></returns>
        public static IEventLogger NewLogger(SourceLevels allowedLogLevels =  SourceLevels.All)
        {
            if (string.IsNullOrEmpty(_defaultLogName) == true)
            {
                _defaultLogName = CoreConfig.ApplicationName;
            }

            return NewLog(_defaultLogName);
        }

        public static IEventLogger NewLogger<T>(SourceLevels allowedLevels = SourceLevels.All)
        {
            return NewLogger(typeof(T), allowedLevels);
        }

        /// <summary>
        /// Create a default logger using full name of type (pattern from log4Net)
        /// </summary>
        /// <param name="logLevels">List of levels (flags) of message severity to log</param>
        /// <param name="type">Type that will be using this logger</param>
        /// <returns>Instance of logger</returns>
        public static IEventLogger NewLogger(Type type, SourceLevels logLevels = SourceLevels.All)
        {
            return NewLogger(type.FullName, logLevels);

        }


        /// <summary>
        /// Create a new log with a specific name.  LoggerLevel is set to 'Vital' (Information, Warning, Error, Critical)  (Default log level is set in .config HashTag.Diagnostics.LogLevel, Default:All)
        /// </summary>/// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <returns>Created log</returns>
        public static IEventLogger NewLog(string logName)
        {
            return NewLogger(logName, CoreConfig.Log.ApplicationLogLevels);
        }

        /// <summary>
        /// Create a new log with a specific name and specific logging level.
        /// </summary>
        /// <param name="logName">Name of log (in logging system).  May not map to actual operating system file name</param>
        /// <param name="allowedSourceLevels">Amount of logging this log will do</param>
        /// <returns>Created log or cached instance if already created one in application domain</returns>
        public static IEventLogger NewLogger(string logName, SourceLevels allowedSourceLevels = SourceLevels.All)
        {
            if (_registeredLogs.ContainsKey(logName) == true)
            {
                _registeredLogs[logName]._logLevels = allowedSourceLevels;
                return _registeredLogs[logName];
            }
            else
            {
                Logger log = new Logger(logName, allowedSourceLevels)
                {
                    Write = OnWrite
                };
                _registeredLogs[logName] = log;
                return log;
            }
        }

        internal static Guid OnWrite(LogMessage evt)
        {
            var convertedEvent = ConvertToEvent(evt);
            _eventProcessor.Submit(convertedEvent);
            return evt.UUID;
        }

        public static IEventLogger NewLogger(object logNameFromObjectsType,SourceLevels allowedSourceLevels=SourceLevels.All)
        {
            return NewLogger(logNameFromObjectsType.GetType(),allowedSourceLevels);
        }


        public static LogEvent ConvertToEvent(LogMessage le)
        {
            var retVal = new LogEvent();
            retVal.Application = le.ApplicationKey;
            retVal.EventDate = le.TimeStamp;
            retVal.EventSource = le.LoggerName;
            retVal.EventType = le.Severity;
            retVal.EventTypeName = le.Severity.ToString();
            retVal.Host = le.MachineName;
            retVal.Message = le.MessageText;
            retVal.User = le.UserIdentity;
            if (le.UserContext != null)
            {
                retVal.User = le.UserContext.DefaultUser;
            }
            retVal.UUID = le.UUID;

            if (le.Exceptions != null && le.Exceptions.Count > 0)
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Exceptions", Value = JsonConvert.SerializeObject(le.Exceptions, Formatting.Indented) });
            }
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Environment", Value = le.ActiveEnvironment });
            if (!string.IsNullOrWhiteSpace(le.ApplicationSubKey))
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "SubKey", Value = le.ApplicationSubKey });
            }
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Priority", Value = ((int)le.Priority).ToString() });
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "PriorityName", Value = le.Priority.ToString() });
            retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "ActivityId", Value = le.ActivityId.ToString() });
            if (le.Reference != null)
            {
                retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Reference", Value = JsonConvert.SerializeObject(le.Reference, Formatting.Indented) });
            }
            if (le.Categories != null)
            {
                foreach (var cat in le.Categories)
                {
                    retVal.Properties.Add(new LogEventProperty() { Group = "General", Name = "Category", Value = cat });
                }
            }
            if (le.Properties != null && le.Properties.Count > 0)
            {
                foreach (var prop in le.Properties)
                {
                    retVal.Properties.Add(new LogEventProperty() { Group = "Properties", Name = prop.Key, Value = prop.Value });
                }
            }

            if (le.HttpContext != null)
            {
                convertToEvent(retVal.Properties, le.HttpContext);
            }
            if (le.MachineContext != null)
            {
                convertToEvent(retVal.Properties, le.MachineContext);
            }
            if (le.UserContext != null)
            {
                convertToEvent(retVal.Properties, le.UserContext);
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
        private static void convertToEvent(List<LogEventProperty> list, Collections.PropertyBag props, string groupName)
        {
            if (props == null || props.Count == 0) return;

            list.AddRange(props.Select(prop => new LogEventProperty() {Group = groupName, Name = prop.Key, Value = prop.Value}));
        }
    }

  
}
