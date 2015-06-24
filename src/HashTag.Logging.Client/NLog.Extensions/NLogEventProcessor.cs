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
namespace NLog.HashTag.Extensions
{
    public class NLogEventProcessor:ILogEventProcessor
    {
        private static ConcurrentDictionary<string, ILoggerBase> _nlogInstances = new ConcurrentDictionary<string, ILoggerBase>();
        private object listLock = new object();
        public Guid Submit(LogEvent evt)
        {
            var logger = getLogger(evt);
            var nLogLevel = getLogLevel(evt);

            if (!logger.IsEnabled(nLogLevel)) return evt.UUID;

            logger.Log((LogEventInfo)mapper(evt,nLogLevel));

            return evt.UUID;
        }

        private LogEventInfo mapper(LogEvent evt,LogLevel nLogLevel)
        {
            var retVal = new LogEventInfo(nLogLevel, evt.EventSource, evt.Message);
            retVal.TimeStamp = evt.TimeStamp;
            retVal.Properties.Add("UUID", evt.UUID);
            retVal.Properties.Add("Application", evt.Application);
            retVal.Properties.Add("Host", evt.Host);
            retVal.Properties.Add("EventType", (int)evt.EventType);
            retVal.Properties.Add("EventTypeName", evt.EventTypeName);
            retVal.Properties.Add("User", evt.User);
            retVal.Properties.Add("EventCode", evt.EventCode);
            retVal.Properties.Add("EventId", evt.EventId);
            
            evt.Properties.ForEach(e => retVal.Properties.Add(string.Format("{0}.{1}", e.Group, e.Name), e.Value));
            return retVal;
        }

        private LogLevel getLogLevel(LogEvent evt)
        {
            switch(evt.EventType)
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
            lock(listLock)
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
            
        }
    }
}
