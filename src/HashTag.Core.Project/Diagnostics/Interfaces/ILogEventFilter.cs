using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HashTag.Diagnostics.Models;

namespace HashTag.Diagnostics
{
    public interface ILogEventFilter:ILogWorker
    {
        bool Matches(LogEvent logEvent);
    }
}
