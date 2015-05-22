using Elmah;
using HashTag.Diagnostics;
using HashTag.Logging.Connector.MSSQL;
using HashTag.Logging.Web.Library.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Web.Library
{
    public class EventRepository:IEventRepository
    {

        public void StoreEvent(LogEvent error)
        {
            var ctx = new dbEventContext();

            var dbEvent = new dbEvent();
            dbEvent.UUID = error.UUID.ToString();
            dbEvent.AllJson = JsonConvert.SerializeObject(error, Formatting.Indented);
            dbEvent.Application = error.ApplicationKey;
            dbEvent.Categories = JsonConvert.SerializeObject(error.Categories, Formatting.Indented);
            dbEvent.Channel = error.LoggerName;
            dbEvent.CorrelationUUID = error.ActivityId.ToString();
            dbEvent.Environment = error.ActiveEnvironment;
            dbEvent.EventCode = error.EventCode;
            dbEvent.EventDate = error.TimeStamp;
            dbEvent.EventId = error.EventId;
            dbEvent.Exceptions = JsonConvert.SerializeObject(error.Exceptions, Formatting.Indented);
            dbEvent.HostName = error.MachineName;
            dbEvent.HttpContext = JsonConvert.SerializeObject(error.HttpContext,Formatting.Indented);
            dbEvent.UserContext = "???";
            dbEvent.MachineContext = JsonConvert.SerializeObject(error.MachineContext,Formatting.Indented);
            dbEvent.Message = error.MessageText;
            dbEvent.Module = error.ApplicationSubKey;
            dbEvent.PriorityCode = error.Priority.ToString();
            dbEvent.PriorityValue = (int)error.Priority;
            dbEvent.Properties = JsonConvert.SerializeObject(error.Properties, Formatting.Indented);
            dbEvent.Reference = error.Reference!=null?error.Reference.ToString():(string)null;
            dbEvent.SeverityCode = error.Severity.ToString();
            dbEvent.SeverityValue = (int)error.Severity;
            dbEvent.Title = error.Title;
            dbEvent.UserContext = JsonConvert.SerializeObject(error.UserContext, Formatting.Indented);
            
            ctx.Events.Add(dbEvent);

            ctx.SaveChanges();
        }

        public void StoreEvent(Error error)
        {
            throw new NotImplementedException();
        }
    }
}
