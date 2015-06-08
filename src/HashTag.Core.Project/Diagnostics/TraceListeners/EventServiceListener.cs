using HashTag.Configuration;
using HashTag.Diagnostics.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics.TraceListeners
{
    public class EventServiceListener : TraceListener
    {

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }
        private string _connectionStringName { get; set; }
        private string _connectionString { get; set; }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data == null) return;

            if (string.IsNullOrWhiteSpace(_connectionStringName))
            {
                foreach (DictionaryEntry entry in Attributes)
                {
                    switch (entry.Key.ToString().ToUpper())
                    {
                        case "CONNECTIONSTRINGNAME": _connectionStringName = entry.Value.ToString(); break;
                    }
                }
                if (string.IsNullOrWhiteSpace(_connectionStringName))
                {
                    throw new ConfigurationErrorsException("Required 'connectionStringName' attribute on listener is missing or has an empty value");
                }
                if (ConfigurationManager.ConnectionStrings[_connectionStringName] == null)
                {
                    throw new ConfigurationErrorsException("Required connection string '{0}' is missing or has an empty value");
                }
                _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new ConfigurationErrorsException("Required connection string '{0}' is missing or has an empty value");
                }
            }

            if (!(data is List<LogEvent>))
            {
                throw new ArgumentException("'data' parameter is expected to be List<LogEvent>.  Received " + data.GetType().FullName);
            }

            sendDataToEventService(data as List<LogEvent>).Wait();

        }

        private async Task sendDataToEventService(List<LogEvent> lm)
        {

            var eventList = lm.ConvertAll<Event>(logEvent => convertToApiEvent(logEvent)).ToList();
            using (var client = new HttpClient())
            {
                var msg = new HttpRequestMessage(HttpMethod.Post, _connectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync(msg.RequestUri, eventList);
                response.EnsureSuccessStatusCode();
            }
        }

        private Event convertToApiEvent(LogEvent le)
        {
            var retVal = new Event();
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
                retVal.Properties.Add(new EventProperty() { Group = "General", Name = "Exceptions", Value = JsonConvert.SerializeObject(le.Exceptions, Formatting.Indented) });
            }
            retVal.Properties.Add(new EventProperty() { Group = "General", Name = "Environment", Value = le.ActiveEnvironment });
            if (!string.IsNullOrWhiteSpace(le.ApplicationSubKey))
            {
                retVal.Properties.Add(new EventProperty() { Group = "General", Name = "SubKey", Value = le.ApplicationSubKey });
            }
            retVal.Properties.Add(new EventProperty() { Group = "General", Name = "Priority", Value = ((int)le.Priority).ToString() });
            retVal.Properties.Add(new EventProperty() { Group = "General", Name = "PriorityName", Value = le.Priority.ToString() });
            retVal.Properties.Add(new EventProperty() { Group = "General", Name = "ActivityId", Value = le.ActivityId.ToString() });
            if (le.Reference != null)
            {
                retVal.Properties.Add(new EventProperty() { Group = "General", Name = "Reference", Value = JsonConvert.SerializeObject(le.Reference, Formatting.Indented) });
            }
            if (le.Categories != null)
            {
                foreach (var cat in le.Categories)
                {
                    retVal.Properties.Add(new EventProperty() { Group = "General", Name = "Category", Value = cat });
                }
            }
            if (le.Properties != null && le.Properties.Count > 0)
            {
                foreach (var prop in le.Properties)
                {
                    retVal.Properties.Add(new EventProperty() { Group = "Properties", Name = prop.Key, Value = prop.Value });
                }
            }

            if (le.HttpContext != null)
            {
                convertToApiEvent(retVal.Properties, le.HttpContext);
            }
            if (le.MachineContext != null)
            {
                convertToApiEvent(retVal.Properties, le.MachineContext);
            }
            if (le.UserContext != null)
            {
                convertToApiEvent(retVal.Properties, le.UserContext);
            }

            retVal.Properties.ForEach(p =>
            {
                p.Event = retVal;
                p.EventUUID = retVal.UUID;
            });
            return retVal;
        }

        private void convertToApiEvent(List<EventProperty> list, LogUserContext logUserContext)
        {
            if (!string.IsNullOrWhiteSpace(logUserContext.AppDomainIdentity))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "AppDomainIdentity", Value = logUserContext.AppDomainIdentity });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.DefaultUser))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "DefaultUser", Value = logUserContext.DefaultUser });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.EnvUserName))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "EnvUserName", Value = logUserContext.EnvUserName });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.HttpUser))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "HttpUser", Value = logUserContext.HttpUser });
            }
            if (logUserContext.IsInteractive.HasValue)
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "IsInteractive", Value = logUserContext.IsInteractive.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logUserContext.ThreadPrincipal))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "ThreadPrincipal", Value = logUserContext.ThreadPrincipal });
            }
            if (!string.IsNullOrWhiteSpace(logUserContext.UserDomain))
            {
                list.Add(new EventProperty() { Group = "UserContext", Name = "UserDomain", Value = logUserContext.UserDomain });
            }
        }
        private const int BytesInMB = (1024 * 1024 * 1024);
        private void convertToApiEvent(List<EventProperty> list, LogMachineContext logMachineContext)
        {
            if (!string.IsNullOrWhiteSpace(logMachineContext.AppDomainName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "AppDomainName", Value = logMachineContext.AppDomainName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.AppFolder))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "AppFolder", Value = logMachineContext.AppFolder });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ClassName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "ClassName", Value = logMachineContext.ClassName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.CommandLine))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "CommandLine", Value = logMachineContext.CommandLine });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAppIdentity))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainAppIdentity", Value = logMachineContext.DomainAppIdentity });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAppName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainAppName", Value = logMachineContext.DomainAppName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAssmName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainAssmName", Value = logMachineContext.DomainAssmName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainAssmVersion))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainAssmVersion", Value = logMachineContext.DomainAssmVersion });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainConfigFile))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainConfigFile", Value = logMachineContext.DomainConfigFile });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.DomainCtxIdentity))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainCtxIdentity", Value = logMachineContext.DomainCtxIdentity });
            }
            if (logMachineContext.DomainId.HasValue)
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "DomainId", Value = logMachineContext.DomainId.Value.ToString() });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.HostName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "HostName", Value = logMachineContext.HostName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.IPAddressList))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "IPAddressList", Value = logMachineContext.IPAddressList });
            }

            if (logMachineContext.Is64BitOperatingSystem.HasValue)
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "Is64BitOperatingSystem", Value = logMachineContext.Is64BitOperatingSystem.Value.ToString() });
            }
            if (logMachineContext.Is64BitProcess.HasValue)
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "Is64BitProcess", Value = logMachineContext.Is64BitProcess.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logMachineContext.ManagedThreadName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "ManagedThreadName", Value = logMachineContext.ManagedThreadName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.MethodName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "MethodName", Value = logMachineContext.MethodName });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.OsVersion))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "OsVersion", Value = logMachineContext.OsVersion });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ProcessId))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "ProcessId", Value = logMachineContext.ProcessId });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.ProcessName))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "ProcessName", Value = logMachineContext.ProcessName });
            }

            if (logMachineContext.ProcessorCount.HasValue)
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "ProcessorCount", Value = logMachineContext.ProcessorCount.Value.ToString() });
            }

            if (!string.IsNullOrWhiteSpace(logMachineContext.StackTrace))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "StackTrace", Value = logMachineContext.StackTrace });
            }
            if (!string.IsNullOrWhiteSpace(logMachineContext.Win32ThreadId))
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "Win32ThreadId", Value = logMachineContext.Win32ThreadId });
            }
            if (logMachineContext.WorkingMemoryBytes.HasValue)
            {
                list.Add(new EventProperty() { Group = "MachineContext", Name = "WorkingMemoryMB", Value = string.Format("{0:0.000}", logMachineContext.WorkingMemoryBytes.Value / BytesInMB) });
            }



        }

        private void convertToApiEvent(List<EventProperty> list, LogHttpContext logHttpContext)
        {
            convertToApiEvent(list, logHttpContext.ServerVariables, "HttpContext.Server");
            convertToApiEvent(list, logHttpContext.Cookies, "HttpContext.Cookies");
            convertToApiEvent(list, logHttpContext.Form, "HttpContext.Form");
            convertToApiEvent(list, logHttpContext.Headers, "HttpContext.Headers");
            convertToApiEvent(list, logHttpContext.QueryString, "HttpContext.QueryString");
        }
        private void convertToApiEvent(List<EventProperty> list, Collections.PropertyBag props, string groupName)
        {
            if (props == null || props.Count == 0) return;

            foreach(var prop in props)
            {
                list.Add(new EventProperty() { Group = groupName, Name = prop.Key, Value = prop.Value });
            }
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();

        }
        protected override string[] GetSupportedAttributes()
        {
            return new string[] {
                "connectionStringName"
            };
        }
    }
}
