using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;

namespace HashTag.Diagnostics.MEX
{
    public class EventSaveItem
    {
        public EventSaveItem(int ordinal=0,HttpStatusCode statusCode = HttpStatusCode.Unused)
        {
            Ordinal = ordinal;
            StatusCode = statusCode;

        }
        public int Ordinal { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Guid EventUUID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
    public class EventSaveResponse
    {
        public EventSaveResponse()
        {
            Results = new List<EventSaveItem>();
        }
        public List<EventSaveItem> Results { get; set; }
    }
}
