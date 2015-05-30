using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using HashTag.Logging.Connector.MSSQL;
using System.Web.OData.Query;
using System.Diagnostics;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.OData._4
{
    [ODataRoutePrefix("4")]
    public class EventsOdataController : ODataController
    {
        EventContext _ctx = new EventContext();

        [ODataRoute]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All,PageSize=20)]
        public IQueryable<Event> Get()
        {          
            return _ctx.Events;
        }

    }
}