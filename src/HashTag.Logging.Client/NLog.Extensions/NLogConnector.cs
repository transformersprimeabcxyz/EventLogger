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
    public class NLogEventConnector : ILogStoreConnector
    {
        private static ConcurrentDictionary<string, ILoggerBase> _nlogInstances = new ConcurrentDictionary<string, ILoggerBase>();
        private object listLock = new object();

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
                for (int x = 0; x < evt.Exceptions.Count; x++)
                {
                    var ex = evt.Exceptions[0];
                    var baseEx = ex.BaseException;
                    if (string.Compare(ex.Message, baseEx.Message) == 0)
                    {
                        retVal.Properties.Add(string.Format("Exception[{0}].Message", x), ex.Message);
                        retVal.Properties.Add(string.Format("Exception[{0}].Source", x), ex.Source);
                        retVal.Properties.Add(string.Format("Exception[{0}].Type", x), ex.ExceptionType);
                        retVal.Properties.Add(string.Format("Exception[{0}].Site", x), ex.TargetSite);
                    }
                    else
                    {
                        retVal.Properties.Add(string.Format("Exception[{0}].Base.Message", x), baseEx.Message);
                        retVal.Properties.Add(string.Format("Exception[{0}].Base.Source", x), baseEx.Source);
                        retVal.Properties.Add(string.Format("Exception[{0}].Base.Type", x), baseEx.ExceptionType);
                        retVal.Properties.Add(string.Format("Exception[{0}].Base.Site", x), baseEx.TargetSite);
                    }
                }
                retVal.Properties.Add("Exceptions", JsonConvert.SerializeObject(evt.Exceptions, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            }
            evt.Properties.ForEach(e => retVal.Properties.Add(string.Format("{0}.{1}", e.Group, e.Name), e.Value));
            return retVal;
        }

        private LogLevel getLogLevel(LogEvent evt)
        {
            switch (evt.EventType)
            {
                case TraceEventType.Critical: return LogLevel.Fatal;
                case TraceEventType.Error: return LogLevel.Error;
                case TraceEventType.Warning: return LogLevel.Warn;
                case TraceEventType.Information: return LogLevel.Info;
                case TraceEventType.Verbose: return LogLevel.Debug;
                case TraceEventType.Start: return LogLevel.Trace;
                case TraceEventType.Stop: return LogLevel.Trace;
                case TraceEventType.Resume: return LogLevel.Trace;
                case TraceEventType.Suspend: return LogLevel.Trace;
                case TraceEventType.Transfer: return LogLevel.Trace;
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

        public void Initialize(IDictionary<string, string> config)
        {
            var nConfig = new LoggingConfiguration();
        }
    }
}
