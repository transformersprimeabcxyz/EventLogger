using HashTag.Diagnostics;
using HashTag.Diagnostics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Client.TraceSource.Extensions
{
    public class TraceSourceConnector:ILogStoreConnector
    {

        public Guid Submit(LogEvent evt)
        {
            return Guid.Empty;
        }

        public void Submit(List<LogEvent> events)
        {
           
        }

        public void Flush()
        {
           
        }

        public void Stop()
        {
           
        }

        public void Start()
        {
           
        }

        public void Initialize(IDictionary<string, string> config)
        {
           
        }
    }
}
