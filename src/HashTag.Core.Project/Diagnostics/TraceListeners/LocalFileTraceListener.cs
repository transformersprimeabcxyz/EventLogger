using HashTag.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class InternalFileTraceListener : TraceListener
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

            internalWrite(eventType, format, args);

        }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data == null) return;

            if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }
            internalWrite(eventType, data.ToString());
        }

        private void internalWrite(TraceEventType eventType, string message, params object[] args)
        {
            var msg = string.Format("{0:yyyy-MM-dd HH:mm:ss.ffff} {1,-12}{2}", DateTime.Now, eventType.ToString(), TextUtils.StringFormat(CultureInfo.InvariantCulture, message, args).Replace(Environment.NewLine, Environment.NewLine + "    "));

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
                    File.AppendAllText(LogFileName,msg + Environment.NewLine);
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
            internalWrite(TraceEventType.Information, message);
        }
        public void WriteLine(string message, params object[] args)
        {
            internalWrite(TraceEventType.Information, message, args);
        }

    }
}
