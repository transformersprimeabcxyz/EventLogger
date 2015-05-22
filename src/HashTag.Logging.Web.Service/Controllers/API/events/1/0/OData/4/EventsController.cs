using HashTag.Diagnostics;
using HashTag.Logging.Connector.MSSQL;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Query;
using System.Web.OData.Routing;

namespace HashTag.Logging.Web.Service.Controllers.API.events._1._0.OData._4
{
    [ODataRoutePrefix("events")]
    public class EventsOdataController : ODataController
    {
        [ODataRoute]
        [EnableQuery(PageSize = 5, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<LogEvent> GetMessages(ODataQueryOptions<LogEvent> opts)
        {
            
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<dbEvent>("dbOdataEvents").EntityType.HasKey(p => p.EventId);
            var m = builder.GetEdmModel();
            //ODataQueryContext dbQueryCtx = new ODataQueryContext(null,null,null);
          //  var op32 = new ODataQueryOptions<dbEvent>(dbQueryCtx, base.Request);

            //var newOpts = QueryOptions<dbEvent>(base.Request.RequestUri.Query);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, base.Request.RequestUri);
            ODataQueryContext context = new ODataQueryContext(m,typeof(dbEvent),null);
            ODataQueryOptions options = new ODataQueryOptions(context, request);
            var ctx = new dbEventContext();

            var result = options.ApplyTo(ctx.Events).Cast<dbEvent>().ToList();

            var r4 = result.Select(s => mapper(s)).ToList();


            return (IQueryable<LogEvent>) opts.ApplyTo(ctx.Events.Select(e => JsonConvert.DeserializeObject<LogEvent>(e.AllJson)) );
           
 
        }
        private LogEvent mapper(dbEvent dbEvent)
        {
            return new LogEvent()
            {

            };
        }
        private static IEdmModel _model;
        private static IEdmModel Model
        {
            get
            {
                if (_model == null)
                {
                    var builder = new ODataConventionModelBuilder();

                    var baseType = typeof(dbEventContext);
                    var sets = baseType.GetProperties().Where(c => c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>));
                    var entitySetMethod = builder.GetType().GetMethod("EntitySet");
                    foreach (var set in sets)
                    {
                        var genericMethod = entitySetMethod.MakeGenericMethod(set.PropertyType.GetGenericArguments());
                        genericMethod.Invoke(builder, new object[] { set.Name });
                    }

                    _model = builder.GetEdmModel();
                }

                return _model;
            }
        }

        //public static ODataQueryOptions<T> QueryOptions<T>(string query = null)
        //{
        //    query = query ?? "";
        //    var url = "http://localhost/Test?" + query;
        //    var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        //    return new ODataQueryOptions<T>(new ODataQueryContext(Model, typeof(T),"odata/events"), request);
        //}
    }
}
