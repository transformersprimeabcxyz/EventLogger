using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HashTag.Diagnostics
{
    public class LogSaveResponse
    {
        public Guid LogEventUUID { get; set; }
        public HttpStatusCode  Status { get; set; }
        public string Message { get; set; }
    }
}
