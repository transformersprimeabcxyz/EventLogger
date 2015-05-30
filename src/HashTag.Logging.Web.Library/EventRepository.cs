using Elmah;
using HashTag.Collections;
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
    public class EventRepository : IEventRepository
    {
        public static AsyncBuffer<LogEvent> _buffer = new AsyncBuffer<LogEvent>(saveEventBlock);

        public void StoreEvent(LogEvent error)
        {
            _buffer.Submit(error);

        }

        public static void saveEventBlock(List<LogEvent> eventBlock)
        {

            var ctx = new EventContext();

            //for (int x = 0; x < eventBlock.Count; x++)
            //{
            //    var error = eventBlock[x];
            //    var dbEvent = new dbEvent();
            //    dbEvent.UUID = error.UUID.ToString();
            //    dbEvent.EventDate = error.TimeStamp;
            //    dbEvent.Message = error.MessageText;
            //    dbEvent.Application = error.ApplicationKey;
            //    dbEvent.Environment = error.ActiveEnvironment;
            //    dbEvent.Categories = error.Categories != null && error.Categories.Count > 0 ? JsonConvert.SerializeObject(error.Categories, Formatting.Indented) : (string)null;
            //    dbEvent.Channel = error.LoggerName;
            //    dbEvent.Module = error.ApplicationSubKey;
            //    dbEvent.CorrelationUUID = error.ActivityId.ToString();
            //    dbEvent.EventCode = error.EventCode;
            //    dbEvent.EventId = error.EventId;
            //    if (error.Exceptions != null && error.Exceptions.Count > 0)
            //    {
            //        var firstEx = error.Exceptions[0];
            //        var baseEx = error.Exceptions[0].GetBaseException;
            //        dbEvent.ExceptionBaseMessage = baseEx.Message;
            //        dbEvent.ExceptionBaseSource = baseEx.Source;
            //        dbEvent.ExceptionBaseType = baseEx.ExceptionType;

            //        dbEvent.ExceptionMessage = firstEx.Message;
            //        dbEvent.ExceptionSource = firstEx.Source;
            //        dbEvent.ExceptionType = firstEx.ExceptionType;

            //        dbEvent.Exceptions = JsonConvert.SerializeObject(error.Exceptions);
            //    }

            //    dbEvent.HostName = error.MachineName;
            //    if (error.HttpContext != null)
            //    {
            //        dbEvent.HttpContext.Cookies = error.HttpContext.Cookies != null && error.HttpContext.Form.Count > 0 ? JsonConvert.SerializeObject(error.HttpContext.Cookies, Formatting.Indented) : (string)null;
            //        dbEvent.HttpContext.Form = error.HttpContext.Form != null && error.HttpContext.Form.Count > 0 ? JsonConvert.SerializeObject(error.HttpContext.Form, Formatting.Indented) : (string)null;
            //        dbEvent.HttpContext.Header = error.HttpContext.Headers != null && error.HttpContext.Headers.Count > 0 ? JsonConvert.SerializeObject(error.HttpContext.Headers, Formatting.Indented) : (string)null;
            //        dbEvent.HttpContext.HtmlMessage = "need this data!";
            //        dbEvent.HttpContext.StatusCode = "need this data!";
            //        dbEvent.HttpContext.StatusValue = -100;
            //        dbEvent.HttpContext.WebEventValue = -100;
            //    }

            //    if (error.MachineContext != null)
            //    {
            //        dbEvent.MachineContext = JsonConvert.SerializeObject(error.MachineContext);
            //    }

            //    dbEvent.PriorityCode = error.Priority.ToString();
            //    dbEvent.PriorityValue = (int)error.Priority;
            //    if (error.Properties != null && error.Properties.Count > 0)
            //    {
            //        dbEvent.Properties = JsonConvert.SerializeObject(error.Properties, Formatting.Indented);
            //    }
            //    dbEvent.Reference = error.Reference != null ? error.Reference.ToString() : (string)null;
            //    dbEvent.SeverityCode = error.Severity.ToString();
            //    dbEvent.SeverityValue = (int)error.Severity;
            //    if (error.UserContext != null)
            //    {
            //        dbEvent.UserContext.AppDomainIdentity = error.UserContext.AppDomainIdentity;
            //        dbEvent.UserContext.EnvUserName = error.UserContext.EnvUserName;
            //        dbEvent.UserContext.HttpUser = error.UserContext.HttpUser;
            //        dbEvent.UserContext.IsInteractive = error.UserContext.IsInteractive;
            //        dbEvent.UserContext.ThreadPrincipal = error.UserContext.ThreadPrincipal;
            //        dbEvent.UserContext.DefaultUser = dbEvent.UserIdentity ?? error.UserContext.DefaultUser;
            //        dbEvent.UserIdentity = dbEvent.UserIdentity ?? error.UserContext.DefaultUser;
            //    }
            //    else
            //    {
            //        dbEvent.UserIdentity = error.UserIdentity;
            //    }

            //    ctx.Events.Add(dbEvent);

                ctx.SaveChanges();
           // }
        }


        public List<LogEvent> GetEvents(string applicationName, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<LogSaveResponse> StoreEvent(List<LogEvent> request)
        {
            //if (request == null || request.Count == 0) return null ;
            //for(int x=0;x<request.Count;x++)
            //{
            //    _buffer.Submit(request[x]);
            //}
            return null;
        }
    }
}
