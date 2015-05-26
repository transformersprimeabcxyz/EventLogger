using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventFilter
    {
        bool ShouldLog(LogEvent logEvent);
    }
}
