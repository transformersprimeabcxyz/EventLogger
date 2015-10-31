using HashTag.Diagnostics;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Client.NLog.Extensions
{
    /// <summary>
    /// NLog target to write Splunk compatible files
    /// </summary>
    [Target("Splunk")]
    public class NLogSplunkTarget:TargetWithLayout
    {
        /// <summary>
        /// dropFolder attribute on target that determines where file(s) should be written to
        /// </summary>
        public string dropFolder { get; set; }

        /// <summary>
        /// Format and output logvent
        /// </summary>
        /// <param name="logEvent">If caller is NLogConnector, <paramref name="logEvent"/></param>.properties will have a complete set of values of HashTag.Diagnostics.LogEvent
        protected override void Write(LogEventInfo logEvent)
        {
            var msg = new Dictionary<object, object>();
            if (logEvent.Exception != null)
            {
                msg["exception"] = logEvent.Exception;
            }

            if (logEvent.HasStackTrace && logEvent.StackTrace != null)
            {
                msg["stackTrace"] = logEvent.StackTrace;
            }

            if (logEvent.UserStackFrame != null)
            {
                msg["userStackFrame"] = logEvent.UserStackFrame;
                msg["userStackFrameIndex"] = logEvent.UserStackFrameNumber;
            }

            foreach(var prop in logEvent.Properties)
            {
                msg[prop.Key] = prop.Value;
            }

            var id = msg["UUID"].ToString();

            var s = JsonConvert.SerializeObject(msg,Formatting.Indented);
            string fileName = Path.Combine(dropFolder, id.ToString()) + ".log";
            File.WriteAllText(fileName, s);
        }        
    }
}
