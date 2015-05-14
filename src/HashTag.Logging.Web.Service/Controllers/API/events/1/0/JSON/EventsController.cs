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
using System.Web.Http.ModelBinding;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.JSON
{
    [RoutePrefix("api/events/0/0/j")]
    public class EventsController : ApiController
    {
        [Route(""),HttpGet]
        public IEnumerable<string> GetStuff()
        {
            var repo = new EventRepository();
            var err = new Error()
            {
                
            };

         //   repo.StoreEvent(err);

            return new string[] { "hello", DateTime.Now.ToString() };
        }

        [Route(""),HttpPost]
        public HttpResponseMessage SaveEvent(LogMessage request)
        {
            return base.Request.CreateResponse<string>(HttpStatusCode.Created, "ehllo");            
        }
    }

    public class ElmahErrorBinder:IModelBinder
    {

        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            return true;
        }
    }
    public class TestRequest
    {
        public TestRequest()
        {
            Vars = new PropertyBag();
            //Vars = new NameValueCollection();
        }
        public string UUID { get; set; }
        public PropertyBag Vars { get; set; }
        
    }
}