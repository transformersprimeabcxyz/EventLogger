using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Writers
{
    public class TraceSourceWriter:ILogEventWriter
    {
        public void Initialize(IDictionary<string, string> config)
        {
            throw new NotImplementedException();
        }

        public bool WriteEvents(List<LogEvent> logEvent)
        {
            if (messageBlock == null || messageBlock.Count == 0)
            {
                return true;
            }

            try
            {
                var ts = TraceSourceFactory.Application;

                foreach (var msg in messageBlock)
                {
                    if (!ts.Switch.ShouldTrace(msg.Severity)) continue;
                    ts.TraceData(msg.Severity, msg.EventId, msg);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Local.Write(ex);
                return false;
            }
        }
    }
}
