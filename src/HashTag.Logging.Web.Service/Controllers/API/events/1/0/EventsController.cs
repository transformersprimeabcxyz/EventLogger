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
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Extensions;
using HashTag.Logging.Service.API.MEX;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0
{
    [RoutePrefix("events/0/0")]
    public class EventsController : ApiController
    {

        EventContext _ctx = new EventContext();

        [Route(""), HttpPost]
        public async Task<HttpResponseMessage> SaveEvent(List<Event> request)
        {

            EventSaveResponse response = null;
            Task.Factory.StartNew(() =>
            {
                var repo = new EventRepository();
                response = repo.SubmitEventList(request);
            }).Wait();
            
            HttpStatusCode  maxStatusCode = (HttpStatusCode) response.Results.Max(r => (int)r.StatusCode);
            return Request.CreateResponse<EventSaveResponse>(maxStatusCode, response);
        }
    }

}