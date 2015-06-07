using HashTag.Collections;
using HashTag.Diagnostics.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    public class LogEventProcessor : ILogEventProcessor,IDisposable
    {
        
        private AsyncBuffer<LogEvent> _buffer;


        public LogEventProcessor()
        {
            _buffer = new AsyncBuffer<LogEvent>(writeEvents)
            {
                BufferSweepMs = 1000,
                CacheTimeOutMs = 15000,
                MaxPageSize = 100
            };
            _buffer.Start();

        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBlock"></param>
        /// <remarks>This is called on a separate thread from _buffer so no need to spin up another thread for persistance.  Each call to this method may be on a different thread</remarks>
        private void writeEvents(List<LogEvent> eventBlock)
        {
            TraceSourceWriter tw = new TraceSourceWriter();
            tw.WriteEvents(eventBlock);
        }

        private bool shouldFlushBuffer(LogEvent le)
        {
            return le.Severity <= TraceEventType.Warning;
        }

        public Guid Submit(LogEvent evt)
        {
            _buffer.Submit(evt);
            if (shouldFlushBuffer(evt))
            {
                Task.Factory.StartNew(() =>
                {
                    Flush();
                }
                );
            }
            return evt.UUID;
        }

        public void Flush()
        {
            _buffer.Flush();
        }

        public void Stop()
        {
            _buffer.Stop();
        }

        public void Start()
        {
            _buffer.Start();
        }

        public void Initialize(IDictionary<string, string> config)
        {

        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _buffer.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~LogEventProcessor()
        {
            Dispose(false);
        }
    }
}
