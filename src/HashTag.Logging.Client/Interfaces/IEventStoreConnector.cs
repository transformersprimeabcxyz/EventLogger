using System;
using System.Collections.Generic;
using HashTag.Diagnostics.Models;

namespace HashTag.Diagnostics
{
    public interface IEventStoreConnector:IEventConnectorBase
    {
        Guid Submit(LogEvent evt);
        void Submit(List<LogEvent> events);
        void Flush();

        void Stop();
        void Start();
        
    }
}
