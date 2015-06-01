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
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Extensions;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0
{
    [RoutePrefix("events/0/0")]
    public class EventsController : ApiController
    {

        EventContext _ctx = new EventContext();

        [Route(""), HttpPost]
        public async Task<HttpResponseMessage> SaveEvent(List<Event> request)
        {
            var repo = new EventRepository();
            EventSaveResponse response = repo.SubmitEventList(request);
            return Request.CreateResponse<EventSaveResponse>(HttpStatusCode.Accepted, response);
        }
    }

}