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

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.OData._4
{
    [ODataRoutePrefix("4")]
    public class EventsOdataController : ODataController
    {        
        [ODataRoute]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public IHttpActionResult Get()
        {
            var list = new List<dbEvent>();
            list.Add(new dbEvent()
            {
                UUID = Guid.NewGuid(),
                 Application = "app",
                 EventDate = DateTime.Now,
                 Host = "specialhost",
                 Message = "we did something",
                 Severity = "Error",
                 User = "bungle one"
            });
            list.Add(new dbEvent()
            {
                UUID = Guid.NewGuid(),
                Application = "app",
                EventDate = DateTime.Now,
                Host = "specialhost",
                Message = "we did something",
                Severity = "Error",
                User = "bungle two"
            });
            list.Add(new dbEvent()
            {
                UUID = Guid.NewGuid(),
                Application = "app",
                EventDate = DateTime.Now,
                Host = "specialhost",
                Message = "we did something",
                Severity = "Error",
                User = "bungle three"
            });
            list[0].Properties.Add(new dbProperty()
            {
                 UUID = Guid.NewGuid(),
                 Event = list[0],
                 EventUUID = list[0].UUID,
                  Name = "exception[0]",
                  Value="System.InvalidFormatException"
            });
            return Ok(list.AsEnumerable());
        }

    }
}