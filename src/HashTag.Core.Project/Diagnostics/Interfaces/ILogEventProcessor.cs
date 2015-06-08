using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventProcessor:ILogWorker
    {
        Guid Submit(LogEvent evt);
        void Submit(List<LogEvent> events);
        void Flush();

        void Stop();
        void Start();
        
    }
}
