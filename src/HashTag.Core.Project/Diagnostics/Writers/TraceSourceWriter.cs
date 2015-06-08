using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Writers
{
    public class TraceSourceWriter : ILogEventWriter
    {
        private IEventLogger _logger = LoggerFactory.NewLogger<TraceSourceWriter>();
        public class Consts
        {
            /// <summary>
            /// 'HashTag.Diagnostics.TraceSource'
            /// </summary>
            public const string DefaultTraceSourceName = "HashTag.Diagnostics.TraceSource";

            /// <summary>
            /// 'TraceSourceName'
            /// </summary>
            public const string Config_TraceSourceNameKey = "TraceSourceName";

        }
        private string _traceSourceName = Consts.DefaultTraceSourceName;

        public void Initialize(IDictionary<string, string> config)
        {
            _traceSourceName = Consts.DefaultTraceSourceName;

            if (config == null) return;
            if (config.ContainsKey(Consts.Config_TraceSourceNameKey))
            {
                _traceSourceName = config[Consts.Config_TraceSourceNameKey];
            }
            _traceSourceName = _traceSourceName ?? Consts.DefaultTraceSourceName;
        }

        public bool WriteEvents(List<LogEvent> eventBlock)
        {
            if (eventBlock == null || eventBlock.Count == 0)
            {
                return true;
            }

            try
            {
                writeToSource(_traceSourceName, eventBlock);
                return true;
            }
            catch (Exception ex)
            {
                var msg = _logger.Error.Catch(ex).Message();
                msg.Fix();
                var evt = LoggerFactory.ConvertToEvent(msg);
                eventBlock.Add(evt);
                var failOverName = string.Format("{0}.{1}", _traceSourceName, "FailOver");
                writeToSource(failOverName, eventBlock);
                return false;
            }
        }

        private void writeToSource(string traceSourceName, List<LogEvent> eventBlock)
        {
            var ts = new TraceSource(traceSourceName);

            ts.Listeners.Remove("Default"); //always remove the 'Default' trace listener

            if (ts.Listeners.Count == 0)
            {
                var tl = new ConsoleTraceListener();
                tl.Name = string.Format("{0}.{1}", traceSourceName, "Listener");
                ts.Listeners.Add(tl);
                ts.Switch = new SourceSwitch(string.Format("{0}.{1}", traceSourceName, "SourceSwitch"), "All");
            }

            var eventsToWrite = eventBlock.Where(evt => ts.Switch.ShouldTrace(evt.EventType)).ToList();
            if (eventsToWrite.Count == 0) return;

            var maxSeverity = (TraceEventType)eventsToWrite.Min(x => (int)x.EventType);

            if (eventsToWrite.Count == 0) return;

            ts.Switch.Level = maxSeverity.ToSourceLevels();
            ts.TraceData(maxSeverity, default(int), eventsToWrite);

        }
    }
}
