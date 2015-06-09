using HashTag.Configuration;
using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.MSSQL
{
    public class dbListener:TraceListener
    {
        
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            EventContext ctx=null;
            try
            {
                if (data == null) return;
                var eventList = data as List<LogEvent>;
                if (eventList == null || eventList.Count == 0) return;
                if (base.Attributes != null && !string.IsNullOrWhiteSpace(base.Attributes["connectionStringName"]))
                {
                    var conifgString = ConfigManager.ConnectionString(base.Attributes["connectionStringName"]);
                    ctx = new EventContext(conifgString);
                }
                else
                {
                    ctx = new EventContext();
                }

                foreach (var logEvent in eventList)
                {
                    var dbEvent = new dbEvent()
                    {
                        Application = logEvent.Application,
                        EventDate = logEvent.EventDate,
                        EventType = logEvent.EventType,
                        EventSource = logEvent.EventSource,
                        EventTypeName = logEvent.EventTypeName,
                        Host = logEvent.Host,
                        Message = logEvent.Message,
                        User = logEvent.User,
                        UUID = logEvent.UUID
                    };


                    foreach (var logProp in logEvent.Properties)
                    {
                        var dbProp = new dbEventProperty()
                        {
                            Event = dbEvent,
                            EventUUID = dbEvent.UUID,
                            Group = logProp.Group,
                            Name = logProp.Name,
                            Value = logProp.Value,
                            UUID = logProp.UUID
                        };
                        dbEvent.Properties.Add(dbProp);
                    }

                    ctx.Events.Add(dbEvent);
                }
                ctx.SaveChangesAsync().Wait();
            }
            finally
            {
                if (ctx != null)
                {
                    ctx.Dispose();
                }
            }
            
            
           
        }
        protected override string[] GetSupportedAttributes()
        {
            return new string[] { "connectionStringName" };
        }
        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }
        
    }
}
