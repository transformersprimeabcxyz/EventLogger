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
        bool WriteBlock(List<LogEvent> logBlock, LogEventProcessorSettings settings);
        void Initialize();
        void WriteEvents(List<LogEvent> logEvent);
        List<string> OnError { get; set; }
        List<string> OnSuccess { get; set; }
        string Name { get; set; }
        JObject Config { get; set; }

    }

    public class TestWriter : LogEventWriterBase
    {

        public void Initialize()
        {

        }

        public void WriteEvents(List<LogEvent> logEvent)
        {

        }









        public JObject Config
        {
            get
            {
                return null;
            }
            set
            {
                Initialize(value);
            }
        }

        public void Initialize(object config)
        {

        }

        protected override void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class LogEventWriterBase : ILogEventWriter
    {

        protected abstract void WriteEvents(List<LogEvent> logBlock, LogEventProcessorSettings settings);

        public virtual void Initialize(object config)
        {

        }
        public List<string> OnError { get; set; }
        public string Name { get; set; }

        public List<string> OnSuccess { get; set; }

        public bool WriteBlock(List<LogEvent> logBlock, LogEventProcessorSettings settings)
        {
            return true;
        }

        public void Initialize()
        {
        }

        public void WriteEvents(List<LogEvent> logEvent)
        {
        }

        public JObject Config
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
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
