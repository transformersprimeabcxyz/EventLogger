using HashTag.Collections;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventWriter : ILogWorker
    {
        bool WriteEvents(List<LogEvent> logEvent);
    }
}
