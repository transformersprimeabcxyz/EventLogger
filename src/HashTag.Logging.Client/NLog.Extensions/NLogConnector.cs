using HashTag.Diagnostics;
using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Collections.Concurrent;
using System.Diagnostics;
using NLog.Config;
using Newtonsoft.Json;
namespace HashTag.Logging.Client.NLog.Extensions
{
    /// <summary>
    /// Connects EventBuilder to NLog
    /// </summary>
    public class NLogEventConnector : IEventStoreConnector
    {
        private static ConcurrentDictionary<string, ILoggerBase> _nlogInstances = new ConcurrentDictionary<string, ILoggerBase>();
        private object listLock = new object();

        /// <summary>
        /// see IEventStoreConnector for documentation
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public Guid Submit(LogEvent evt)
        {
            var logger = getLogger(evt);
            var nLogLevel = getLogLevel(evt);

            if (!logger.IsEnabled(nLogLevel)) return evt.UUID;

            logger.Log((LogEventInfo)mapper(evt, nLogLevel));

            return evt.UUID;
        }

        private LogEventInfo mapper(LogEvent evt, LogLevel nLogLevel)
        {
            var retVal = new LogEventInfo(nLogLevel, evt.EventSource, evt.Message);
            retVal.TimeStamp = evt.TimeStamp;
            retVal.Properties.Add("TimeStamp", evt.TimeStamp);
            retVal.Properties.Add("Message", evt.Message);
            retVal.Properties.Add("UUID", evt.UUID);
            retVal.Properties.Add("Application", evt.Application);
            retVal.Properties.Add("Module", evt.Module);

            retVal.Properties.Add("Environment", evt.Environment);
            retVal.Properties.Add("Host", evt.Host);
            retVal.Properties.Add("EventSource", evt.EventSource);
            retVal.Properties.Add("EventType", evt.EventTypeName);
            retVal.Properties.Add("EventTypeId", (int)evt.EventType);

            retVal.Properties.Add("Priority", evt.PriorityName);
            retVal.Properties.Add("PriorityId", (int)evt.Priority);

            if (!string.IsNullOrWhiteSpace(evt.User)) retVal.Properties.Add("User", evt.User);
            if (!string.IsNullOrWhiteSpace(evt.EventCode)) retVal.Properties.Add("EventCode", evt.EventCode);

            retVal.Properties.Add("EventId", evt.EventId);
            if (evt.Exceptions != null && evt.Exceptions.Count > 0)
            {
                for (int exceptionIndex = 0; exceptionIndex < evt.Exceptions.Count; exceptionIndex++)
                {
                    expandException(evt.Exceptions[exceptionIndex], exceptionIndex, retVal.Properties);
                }               
            }
         
            evt.Properties.ForEach(e => retVal.Properties.Add(string.Format("{0}.{1}", e.Group, e.Name), e.Value));
            return retVal;
        }

        private void expandException(LogException logException, int exceptionIndex, IDictionary<object, object> nlogProperties)
        {
            var baseEx = logException.BaseException;
            if (logException.InnerException != null || logException.BaseException.ExceptionId != logException.ExceptionId)
            {
                nlogProperties.Add(string.Format("Exception[{0}].Base.Message", exceptionIndex), baseEx.Message);                
            }
            expandException(logException, nlogProperties, "", exceptionIndex);
            
            nlogProperties.Add(string.Format("Exception[{0}].JSON", exceptionIndex), JsonConvert.SerializeObject(logException, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        private void expandException(LogException ex, IDictionary<object,object> nlogProperties,string qualifier,int exceptionIndex)
        {
            if (ex.InnerException != null)
            {
                expandException(ex.InnerException, nlogProperties, qualifier + ".", exceptionIndex);
            }
            nlogProperties.Add(string.Format("Exception[{0}].{1}Message", exceptionIndex,qualifier), ex.Message);
            nlogProperties.Add(string.Format("Exception[{0}].{1}StackTrace", exceptionIndex,qualifier), ex.StackTrace);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Source", exceptionIndex,qualifier), ex.Source);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Type", exceptionIndex,qualifier), ex.ExceptionType);
            nlogProperties.Add(string.Format("Exception[{0}].{1}ErrorCode", exceptionIndex,qualifier), ex.ErrorCode);
            nlogProperties.Add(string.Format("Exception[{0}].{1}HelpLink", exceptionIndex,qualifier), ex.HelpLink);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Module", exceptionIndex, qualifier), ex.Module);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Class", exceptionIndex, qualifier), ex.Class);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Method", exceptionIndex, qualifier), ex.Method);
            if (ex.Properties.Count > 0)
            {
                foreach (var exProp in ex.Properties)
                {
                    try
                    {
                        nlogProperties.Add(string.Format("Exception[{0}].{2}{1}", exceptionIndex, exProp.Key, qualifier), exProp.Value);
                    }
                    catch(ArgumentException)
                    {
                        continue;
                    }
                }
            }

            if (ex.Data.Count > 0)
            {
                foreach (var exData in ex.Data)
                {
                    nlogProperties.Add(string.Format("Exception[{0}].{2}Data.{1}", exceptionIndex,exData.Key,qualifier), exData.Value);
                }
            }
            nlogProperties.Add(string.Format("Exception[{0}].{1}Html.EventCode", exceptionIndex,qualifier), ex.HttpWebEventCode);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Html.StatusValue", exceptionIndex,qualifier), ex.HttpStatusValue);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Html.StatusCode", exceptionIndex,qualifier), ex.HttpStatusCode);
            nlogProperties.Add(string.Format("Exception[{0}].{1}Html.Message", exceptionIndex,qualifier), ex.HttpHtmlMessage);

        }
        private LogLevel getLogLevel(LogEvent evt)
        {
            switch (evt.EventType)
            {
                case TraceEventType.Critical: return LogLevel.Fatal;
                case TraceEventType.Error: return LogLevel.Error;
                case TraceEventType.Warning: return LogLevel.Warn;
                case TraceEventType.Information: return LogLevel.Info;
                case TraceEventType.Verbose: return LogLevel.Trace;
                case TraceEventType.Start: return LogLevel.Debug;
                case TraceEventType.Stop: return LogLevel.Debug;
                case TraceEventType.Resume: return LogLevel.Debug;
                case TraceEventType.Suspend: return LogLevel.Debug;
                case TraceEventType.Transfer: return LogLevel.Debug;
            }
            return LogLevel.Off;
        }

        private ILoggerBase getLogger(LogEvent evt)
        {
            var loggerName = getLoggerName(evt);

            if (_nlogInstances.ContainsKey(loggerName))
            {
                return _nlogInstances[loggerName];
            }
            lock (listLock)
            {
                if (_nlogInstances.ContainsKey(loggerName))
                {
                    return _nlogInstances[loggerName];
                }

                var nLogger = LogManager.GetLogger(loggerName);
                _nlogInstances[loggerName] = nLogger;
                return nLogger;
            }
        }

        private string getLoggerName(LogEvent evt)
        {
            if (!string.IsNullOrWhiteSpace(evt.EventSource)) return evt.EventSource;
            if (!string.IsNullOrWhiteSpace(evt.Application)) return evt.Application;
            return "default";
        }

        /// <summary>
        /// See <see cref="HashTag.Diagnostics.IEventBuilder">IEventBuilder</see> for documentation
        /// </summary>
        /// <param name="events"></param>
        public void Submit(List<LogEvent> events)
        {
            events.ForEach(e => Submit(e));
        }

        public void Flush()
        {
            var keys = _nlogInstances.Keys.ToList();
            keys.ForEach(k =>
            {
                _nlogInstances[k].Factory.Flush();
            });
        }

        /// <summary>
        /// See <see cref="HashTag.Diagnostics.IEventBuilder">IEventBuilder</see> for documentation
        /// </summary>
        public void Stop()
        {
            var keys = _nlogInstances.Keys.ToList();
            keys.ForEach(k =>
            {
                var factory = _nlogInstances[k].Factory;
                if (factory.IsLoggingEnabled())
                {
                    factory.SuspendLogging();
                }
            });
        }

        /// <summary>
        /// See <see cref="HashTag.Diagnostics.IEventBuilder">IEventBuilder</see> for documentation
        /// </summary>
        public void Start()
        {
            var keys = _nlogInstances.Keys.ToList();
            keys.ForEach(k =>
            {
                var factory = _nlogInstances[k].Factory;
                if (!factory.IsLoggingEnabled())
                {
                    factory.ResumeLogging();
                }
            });
        }

        /// <summary>
        /// See <see cref="HashTag.Diagnostics.IEventBuilder">IEventBuilder</see> for documentation (no-op)
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(IDictionary<string, string> config)
        {
            //var nConfig = new LoggingConfiguration();
            
        }
    }
}
