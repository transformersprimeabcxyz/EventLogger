using Elmah;
using HashTag.Collections;
using HashTag.Diagnostics;
using HashTag.Logging.Web.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;


namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.JSON
{
    [RoutePrefix("api/events/0/0/j")]
    public class EventsController : ApiController
    {
       [Route(""),HttpPost,ValidateLogEvent]
        public HttpResponseMessage SaveEvent(LogEvent request)
        {
            try
            {
                EventRepository repo = new EventRepository();
                if (request != null && request.UUID == Guid.Empty)
                {
                    request.UUID = Guid.NewGuid();
                }
                if (string.IsNullOrWhiteSpace(request.MessageText) && request.Exceptions != null && request.Exceptions.Count > 0)
                {
                    request.MessageText = request.Exceptions[0].Message;
                }
                repo.StoreEvent(request);
            }
            catch(Exception ex)
            {
                var ss = new LogException(ex);
                var s = JsonConvert.SerializeObject(ex, Formatting.Indented);
            }
            return base.Request.CreateResponse<Guid>(HttpStatusCode.Created, request.UUID);            
        }
    }

    
    public class ValidateLogEventAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}