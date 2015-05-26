using HashTag.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public interface ILogEventWriter
    {
        bool WriteBlock(List<LogEvent> logBlock,LogEventProcessorSettings settings);

        void Submit(LogEvent logEvent);
        
    }
    public abstract class LogEventWriterBase
    {
        private static AsyncBuffer<LogEvent> _buffer = new AsyncBuffer<LogEvent>();

        protected abstract void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings);
       
    }
    public class LogEventWriterREST : LogEventWriterBase
    {
        protected override void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings)
        {
            throw new NotImplementedException();
        }
    }

    public class LogEventWriterSql : LogEventWriterBase
    {
        protected override void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings)
        {
            throw new NotImplementedException();
        }
    }
    public class LogEventWriterFile : LogEventWriterBase
    {
        protected override void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
