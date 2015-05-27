using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class EventGetRequest
    {
        public EventGetRequest()
        {
            host = new string[] { "my host" };
            pageIndex = 100;
        }
        public string[] application { get; set; }
        public Guid?[] id { get; set; }
        public string[] severity { get; set; }
        public string[] env { get; set; }
        public string[] host { get; set; }
        public string[] userName { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
