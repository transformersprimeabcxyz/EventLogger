using HashTag.Diagnostics;
using Newtonsoft.Json;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.HashTag.Extensions
{
    [Target("Splunk")]
    public class SplunkTarget:TargetWithLayout
    {
        public string dropFolder { get; set; }
        protected override void Write(LogEventInfo logEvent)
        {
            var msg = new Dictionary<object, object>();
            msg["timeStamp"] = logEvent.TimeStamp;
            msg["level"] = logEvent.Level;
            msg["logger"] = logEvent.LoggerName;
            msg["message"] = logEvent.FormattedMessage;
            msg["host"] = Environment.MachineName;
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

            if (!logEvent.Properties.ContainsKey((object)"Id")) 
            {
                msg["id"] = Guid.NewGuid();
            }
            else
            {
                msg["id"] = logEvent.Properties["Id"];
                logEvent.Properties.Remove("Id");
            }

            foreach(var prop in logEvent.Properties)
            {
                msg[prop.Key] = prop.Value;
            }

            var id = msg["id"].ToString();
            var s = JsonConvert.SerializeObject(msg,Formatting.None);

            string fileName = Path.Combine(dropFolder, id.ToString()) + ".log";
            File.WriteAllText(fileName, s);
        }        
    }
}
