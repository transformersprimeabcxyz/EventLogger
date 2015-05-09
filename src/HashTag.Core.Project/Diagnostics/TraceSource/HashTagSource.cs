using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public class HashTagSource
    {
        public TraceSource NetSource { get; set; }
        public TraceSourceTypes SourceType { get; set; }
        public string Name { get; set; }
    }
}
