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
using System.Threading.Tasks;
using HashTag.Logging.Web.Library;
using HashTag.Logging.MSSQL;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.OData._4
{
    public class EventsOdataController : ODataController
    {
        EventContext _ctx = new EventContext();

        [ODataRoute("4")]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, PageSize = 100)]
        public async Task<IQueryable<dbEvent>> Get()
        {
            return _ctx.Events;
        }
        [ODataRoute("4({key})")]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public async Task<IHttpActionResult> Get([FromODataUri] Guid key)
        {            
            return Ok(_ctx.Events.Include("Properties").Where(e => e.UUID == key));
        }
    }

}