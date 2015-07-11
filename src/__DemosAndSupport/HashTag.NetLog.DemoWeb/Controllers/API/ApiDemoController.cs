using HashTag.Diagnostics;
using HashTag.NetLog.DemoWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HashTag.NetLog.DemoWeb.Controllers.API
{    
    public class ApiDemoController : ApiController
    {
        IEventLogger _log;
        IInjectedService _svc;
        public ApiDemoController(IEventLogger log,IInjectedService svc)
        {
            _log = log;
            _svc = svc;
        }
        [Route("~/api/demo"),HttpGet]
        public HttpResponseMessage GetMessage()
        {            
            var message = string.Format("Clicked at: {0:HH:mm:ss.fff}", DateTime.Now);
            _log.Info.Write(message);
            _svc.SaveRecords(new List<int>(){1, 2, 3, 4, 5});
            return Request.CreateResponse<string>(HttpStatusCode.OK, message);
        }

        [Route("~/api/demo-with-error"),HttpGet]
        public HttpResponseMessage GetMessageWithUnHandledErrorUsingElmah()
        {         
            //NOTE: By default Web API 2 doesn't route unhandled API exceptions to Elmah.  Use WebApiConfig for work-around
            _svc.SaveRecordsWithRandomError(new List<int>() { 1, 2, 3, 4, 5 });
            return Request.CreateResponse<string>(HttpStatusCode.OK,""); //never called because above line throws unhandled exception
        }

        [Route("~/api/demo-with-error-handled"), HttpGet]
        public HttpResponseMessage GetMessageWithUnHandledError()
        {
            //NOTE: By default Web API 2 doesn't route unhandled API exceptions to Elmah.  Use WebApiConfig for work-around
            try
            {
                _svc.SaveRecordsWithRandomError(new List<int>() { 1, 2, 3, 4, 5 });
                return Request.CreateResponse<string>(HttpStatusCode.OK, ""); //never called because above line throws unhandled exception
            }
            catch(Exception ex)
            {
                _log.Error.Write(ex);
                return Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
            } 
        }
    }
}
