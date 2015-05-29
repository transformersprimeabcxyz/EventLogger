using HashTag.Collections;
using HashTag.Diagnostics;
using HashTag.Logging.Connector.MSSQL;
using HashTag.Logging.Web.Library;
using HashTag.Web.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using HashTag.Diagnostics.MEX;


namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.JSON
{
    [RoutePrefix("events/0/0/j")]
    public class EventsController : ApiController
    {
       [Route(""),HttpPost,ValidateLogEvent]
        public async Task<HttpResponseMessage> SaveEvent(List<LogEvent> request)
        {
            var response = new ApiResponseBase<List<LogSaveResponse>>();
            response.Header.HttpStatus = HttpStatusCode.Accepted;

            try
            {
                EventRepository repo = new EventRepository();
                var saveResponse = repo.StoreEvent(request);
                return base.Request.CreateResponse<ApiResponseBase<List<LogSaveResponse>>>(response.Header.HttpStatus, response);            
            }
            catch(Exception ex)
            {
                response.Header.HttpStatus = HttpStatusCode.InternalServerError;
                response.Header.Messages.Add(ex);
                response.Header.Messages[0].MessageStatus = ApiMessageStatus.Error;
                return base.Request.CreateResponse<ApiResponseBase>(response.Header.HttpStatus, response);
            }
            
        }

        [Route("dummy"), HttpPost, ValidateLogEvent]
        public async Task<HttpResponseMessage> SaveEvent(List<dbEvent> request)
        {
            //var evt = new dbEvent()
            //{
            //    Application = "app name",
            //    EventDate = DateTime.Now,
            //    Host = "host name",
            //    Message = "message",
            //    Severity = TraceEventType.Error.ToString(),
            //    UUID = Guid.NewGuid()

            //};

            //var s = JsonConvert.SerializeObject(evt, Formatting.Indented);
            var response = new EventSaveResponse();
            response.Results.Add(new EventSaveItem()
            {
                 StatusCode = HttpStatusCode.Accepted,
                 Ordinal = request.Count,
                 EventUUID = Guid.NewGuid()
            });
            return base.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "hello");
        }

        [Route("4"), HttpGet, ValidateLogEvent]
       public async Task<HttpResponseMessage> GetEvents([FromUri] EventGetRequest request)
       {
           return null;

       }
    }

    
    public class ValidateLogEventAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            //if (actionContext.ModelState.IsValid == false)
            //{
            //    var response = new ApiResponseBase();
            //    response.Header.HttpStatus = HttpStatusCode.BadRequest;
            //    foreach(var modelError in actionContext.ModelState)
            //    {
            //        var x = modelError.Key;
            //        var y = modelError.Value.Errors[0].ErrorMessage;
            //        response.Header.Messages.Add(ApiMessageStatus.Error,"{0} {1}", x, y);
            //    }
            //    actionContext.Response = actionContext.Request.CreateResponse<ApiResponseBase>(HttpStatusCode.BadRequest, response);
            //}
        }
    }
}