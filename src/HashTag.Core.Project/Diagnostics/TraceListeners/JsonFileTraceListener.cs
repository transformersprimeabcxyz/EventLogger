using HashTag.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class JsonFileTraceListener : TraceListener
    {
        static object _lock = new object();


        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        public string LogFileName { get; set; }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
            {
                return;
            }

            var msg = new LogMessage(format, args)
            {
                 EventId = id,
                  Severity =eventType,
                 
            };
            msg.Categories.Add(source);

            internalWrite(eventType, JsonConvert.SerializeObject(msg,Formatting.Indented));

        }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data == null) return;

            if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }
            internalWrite(eventType, JsonConvert.SerializeObject(data,Formatting.Indented));
        }

        private void internalWrite(TraceEventType eventType, string message)
        {

            lock (_lock)
            {
                try
                {
                    if (!File.Exists(LogFileName))
                    {
                        using (var fs = File.CreateText(LogFileName))
                        {
                            fs.Close();
                        }
                    }
                    File.AppendAllText(LogFileName, message + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Logger.Internal.Write(ex);
                }
            }
        }

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            var msg = new LogMessage(message)
            {
                Severity = TraceEventType.Information
            };

            internalWrite(msg.Severity, JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
        public void WriteLine(string message, params object[] args)
        {
            var msg = new LogMessage(message,args)
            {
                Severity = TraceEventType.Information
            };

            internalWrite(msg.Severity, JsonConvert.SerializeObject(msg, Formatting.Indented));
        }

    }
}
