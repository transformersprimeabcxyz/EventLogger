using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// This is called on an application's child thread
    /// </summary>
    public class TraceSourceConnector:ILogConnector
    {
        public void Initialize()
        {
            Trace.AutoFlush = true;

        }

        public bool PersistMessages(List<LogEvent> messageBlock)
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

        public bool Flush()
        {
            return true;
        }

        public bool Close()
        {
            return true;
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {

            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~TraceSourceConnector()
        {
            Dispose(false);
        }
    }
}
