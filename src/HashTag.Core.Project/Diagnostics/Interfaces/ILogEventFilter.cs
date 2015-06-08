using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventFilter:ILogWorker
    {
        bool Matches(LogMessage logEvent);
    }
}
