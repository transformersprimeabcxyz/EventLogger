using HashTag.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    public class LogEventProcessor : ILogEventProcessor
    {
        private LogEventProcessorSettings _settings;
        
        private AsyncBuffer<LogEvent> _buffer;


        public LogEventProcessor()
        {
            _buffer = new AsyncBuffer<LogEvent>(writeEvents);

        }
        public LogEventProcessor(LogEventProcessorSettings config)
        {
           
        }

        private void writeEvents(List<LogEvent> eventBlock)
        {
            var writingTask = Task.Factory.StartNew(() =>
                {
                    removeUnqualifiedEvents(eventBlock, _settings.ShouldLogEventFilters);
                    foreach (ILogEventWriter writer in _settings.Pipeline)
                    {
                        if (writer.WriteEvents(eventBlock) == true) break;
                    }
                });
            writingTask.Wait(_settings.Processor.BufferWriteTimeOutMs);
        }

        private static void removeUnqualifiedEvents(List<LogEvent> eventBlock, List<ILogEventFilter> filters)
        {
            if (filters == null || filters.Count == 0) return;

            if (eventBlock == null || eventBlock.Count == 0) return;

            foreach (var filter in filters)
            {
                eventBlock.RemoveAll(evt => !filter.Matches(evt));
            }
        }

        private bool shouldFlushBuffer(LogEvent le, List<ILogEventFilter> filters)
        {
            if (le == null || filters == null || filters.Count == 0) return true;
            return filters.Any(filter => filter.Matches(le) == true);
        }

        public Guid Submit(LogEvent evt)
        {
            _buffer.Submit(evt);
            if (shouldFlushBuffer(evt, _settings.Processor.ForceFlushFilters.ToList()))
            {
                Flush();
            }
            return evt.UUID;
        }

        public void Flush()
        {
            _buffer.Flush();
        }

        public void Stop()
        {
            _buffer.Stop();
        }

        public void Start()
        {
            _buffer.Start();
        }

        public void Initialize(IDictionary<string,string> config)
        {
            //if (!(config is JObject)) return;

            //var serializer = new Newtonsoft.Json.JsonSerializer();
            //serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;

            //_settings = ((JObject)config).ToObject<LogEventProcessorSettings>(serializer);
            //_buffer = new AsyncBuffer<LogEvent>();
            //_buffer.MaxPageSize = _settings.Processor.BufferBlockSize;
            //_buffer.BufferSweepMs = _settings.Processor.BufferSweepMs;
            //_buffer.CacheTimeOutMs = _settings.Processor.CacheTimeOutMs;
            //_buffer.Start();
        }
    }

    public class LogEventProcessorSettings
    {
        public LogEventProcessorSettings()
        {
            Pipeline = new List<ILogEventWriter>();
            ShouldLogEventFilters = new List<ILogEventFilter>();
            Processor = new LogEventProcessorBufferSettings();
        }

        public LogEventProcessorBufferSettings Processor { get; set; }
        public List<ILogEventWriter> Pipeline { get; set; }
        public List<ILogEventFilter> ShouldLogEventFilters { get; set; }

    }
    public class LogEventProcessorBufferSettings
    {
        public LogEventProcessorBufferSettings()
        {
            // ForceFlushFilters = new List<ILogEventFilter>();
            ForceFlushFilters = new ILogEventFilter[] { };
        }
        public int BufferBlockSize { get; set; }
        public int BufferSweepMs { get; set; }
        public int CacheTimeOutMs { get; set; }
        public int BufferWriteTimeOutMs { get; set; }
        public ILogEventFilter[] ForceFlushFilters { get; set; }
    }
}
