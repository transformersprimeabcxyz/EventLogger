using Elmah;
using HashTag.Collections;
using HashTag.Diagnostics;
using HashTag.Logging.Web.Library;
using HashTag.Web.Api;
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
            var response = new ApiResponseBase<Guid>();
            response.Header.HttpStatus = HttpStatusCode.Accepted;
            response.Body = request.UUID;

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
                response.Header.Links.Add(base.Request.RequestUri.ToString()+"/"+request.UUID.ToString(), "self", request.UUID.ToString());
                return base.Request.CreateResponse<ApiResponseBase<Guid>>(response.Header.HttpStatus, response);            
            }
            catch(Exception ex)
            {
                response.Header.HttpStatus = HttpStatusCode.InternalServerError;
                response.Header.Messages.Add(ex);
                response.Header.Messages[0].MessageStatus = ApiMessageStatus.Error;
                return base.Request.CreateResponse<ApiResponseBase>(response.Header.HttpStatus, response);
            }
            
        }
    }

    
    public class ValidateLogEventAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            if (actionContext.ModelState.IsValid == false)
            {
                var response = new ApiResponseBase();
                response.Header.HttpStatus = HttpStatusCode.BadRequest;
                foreach(var modelError in actionContext.ModelState)
                {
                    var x = modelError.Key;
                    var y = modelError.Value.Errors[0].ErrorMessage;
                    response.Header.Messages.Add(ApiMessageStatus.Error,"{0} {1}", x, y);
                }
                actionContext.Response = actionContext.Request.CreateResponse<ApiResponseBase>(HttpStatusCode.BadRequest, response);
            }
        }
    }
}