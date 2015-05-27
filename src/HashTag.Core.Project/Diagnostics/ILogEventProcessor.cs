using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventProcessor:ILogWorker
    {
        Guid Submit(LogEvent evt);

        void Flush();

        void Stop();
        void Start();
        
    }
}
